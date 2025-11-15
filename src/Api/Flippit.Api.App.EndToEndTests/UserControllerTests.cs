using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Flippit.Common.Enums;
using Flippit.Common.Models.Collection;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Common.Models.User;
using Xunit;

namespace Flippit.Api.App.EndToEndTests
{
    public class UserControllerTests : IAsyncDisposable
    {
        private readonly FlippitApiApplicationFactory application;
        private readonly Lazy<HttpClient> client;
        private readonly JsonSerializerOptions jsonOptions;

        public UserControllerTests()
        {
            application = new FlippitApiApplicationFactory();
            client = new Lazy<HttpClient>(application.CreateClient());
            jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
            };
        }

        [Fact]
        public async Task GetAllUsers_Returns_At_Least_One_User()
        {
            var response = await client.Value.GetAsync("/api/users");

            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<ICollection<UserListModel>>(jsonOptions);
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task GetAllUsers_WithFilter_Returns_Filtered_Users()
        {
            var filter = "Test";

            var response = await client.Value.GetAsync($"/api/users?filter={filter}");

            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<ICollection<UserListModel>>(jsonOptions);
            Assert.NotNull(users);
            Assert.All(users, user => Assert.Contains(filter, user.Name, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetAllUsers_WithSortByName_Returns_Sorted_Users()
        {
            var response = await client.Value.GetAsync("/api/users?sortBy=name");

            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<ICollection<UserListModel>>(jsonOptions);
            Assert.NotNull(users);
            Assert.NotEmpty(users);
            var sortedNames = users.Select(u => u.Name).ToList();
            var expectedOrder = sortedNames.OrderBy(n => n).ToList();
            Assert.Equal(expectedOrder, sortedNames);
        }

        [Fact]
        public async Task GetUsers_WithPaging_Returns_Correct_Page_Size()
        {
            int pageSize = 5;
            for (int i = 0; i < pageSize + 2; i++)
            {
                var user = new UserDetailModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"User {i}",
                    Role = Role.User
                };
                var response = await client.Value.PostAsJsonAsync("/api/users", user, jsonOptions);
                response.EnsureSuccessStatusCode();
            }

            var getResponse = await client.Value.GetAsync($"/api/users?pageSize={pageSize}&page=1");
            getResponse.EnsureSuccessStatusCode();
            var users = await getResponse.Content.ReadFromJsonAsync<ICollection<UserListModel>>(jsonOptions);

            Assert.NotNull(users);
            Assert.NotEmpty(users);
            Assert.True(users.Count <= pageSize, "Returned users exceed page size");
        }

        [Fact]
        public async Task GetUserById_Returns_Correct_User()
        {
            var newUser = new UserDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                PhotoUrl = "http://example.com/photo.jpg",
                Role = Role.User
            };
            var createResponse = await client.Value.PostAsJsonAsync("/api/users", newUser, jsonOptions);
            createResponse.EnsureSuccessStatusCode();
            var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>(jsonOptions);

            var response = await client.Value.GetAsync($"/api/users/{createdId}");

            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<UserDetailModel>(jsonOptions);
            Assert.NotNull(user);
            Assert.Equal(createdId, user.Id);
            Assert.Equal(newUser.Name, user.Name);
        }


        [Fact]
        public async Task GetUserById_NonExistentId_Returns_NotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var response = await client.Value.GetAsync($"/api/users/{nonExistentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCollectionsByUserId_Returns_Collections()
        {
            var userId = Guid.NewGuid();

            var response = await client.Value.GetAsync($"/api/users/{userId}/collections");

            response.EnsureSuccessStatusCode();
            var collections = await response.Content.ReadFromJsonAsync<ICollection<CollectionListModel>>(jsonOptions);
            Assert.NotNull(collections);
        }

        [Fact]
        public async Task GetCompletedLessonsByUserId_Returns_Lessons()
        {
            var userId = Guid.NewGuid();

            var response = await client.Value.GetAsync($"/api/users/{userId}/CompletedLessons");

            response.EnsureSuccessStatusCode();
            var lessons = await response.Content.ReadFromJsonAsync<ICollection<CompletedLessonListModel>>(jsonOptions);
            Assert.NotNull(lessons);
        }

        [Fact]
        public async Task CreateUser_Returns_Created_User_Id()
        {
            var newUser = new UserDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                PhotoUrl = "http://example.com/photo.jpg",
                Role = Role.User
            };

            var response = await client.Value.PostAsJsonAsync("/api/users", newUser, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var createdId = await response.Content.ReadFromJsonAsync<Guid>(jsonOptions);
            Assert.NotEqual(Guid.Empty, createdId);
            Assert.Equal(newUser.Id, createdId);
        }

        [Fact]
        public async Task UpdateUser_Returns_Updated_User_Id()
        {
            var newUser = new UserDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                PhotoUrl = "http://example.com/photo.jpg",
                Role = Role.User
            };

            var createResponse = await client.Value.PostAsJsonAsync("/api/users", newUser, jsonOptions);
            createResponse.EnsureSuccessStatusCode();
            var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>(jsonOptions);

            var updatedUser = new UserDetailModel
            {
                Id = createdId,
                Name = "Updated User",
                PhotoUrl = "http://example.com/updated_photo.jpg",
                Role = Role.Admin
            };

            var response = await client.Value.PutAsJsonAsync("/api/users", updatedUser, jsonOptions);
            response.EnsureSuccessStatusCode();
            var returnedId = await response.Content.ReadFromJsonAsync<Guid?>(jsonOptions);
            Assert.Equal(createdId, returnedId);

            var getResponse = await client.Value.GetAsync($"/api/users/{createdId}");
            getResponse.EnsureSuccessStatusCode();
            var userFromApi = await getResponse.Content.ReadFromJsonAsync<UserDetailModel>(jsonOptions);

            Assert.Equal("Updated User", userFromApi!.Name);
            Assert.Equal("http://example.com/updated_photo.jpg", userFromApi.PhotoUrl);
            Assert.Equal(Role.Admin, userFromApi.Role);

            var allUsersResponse = await client.Value.GetAsync("/api/users");
            allUsersResponse.EnsureSuccessStatusCode();
            var allUsers = await allUsersResponse.Content.ReadFromJsonAsync<ICollection<UserListModel>>(jsonOptions);

            Assert.Single(allUsers!, u => u.Id == createdId);
        }


        [Fact]
        public async Task UpdateUser_NonExistentId_Returns_Ok_With_Null()
        {
            var nonExistentId = Guid.NewGuid();
            var updatedUser = new UserDetailModel
            {
                Id = nonExistentId,
                Name = "Non-Existent User",
                PhotoUrl = "http://example.com/photo.jpg",
                Role = Role.User
            };

            var response = await client.Value.PutAsJsonAsync("/api/users", updatedUser, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(string.IsNullOrEmpty(content) || content == "null", "Expected empty or null response body");
        }

        [Fact]
        public async Task UpdateUser_WithInvalidData_Returns_BadRequest()
        {
            var userId = Guid.NewGuid();
            var invalidUser = new UserDetailModel
            {
                Id = userId,
                Name = "",
                PhotoUrl = "http://example.com/photo.jpg",
                Role = Role.User
            };

            var response = await client.Value.PutAsJsonAsync("/api/users", invalidUser, jsonOptions);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpsertUser_Creates_New_User()
        {
            var newUser = new UserDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "New User",
                PhotoUrl = "http://example.com/new_photo.jpg",
                Role = Role.User
            };

            var response = await client.Value.PostAsJsonAsync("/api/users/upsert", newUser, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var createdId = await response.Content.ReadFromJsonAsync<Guid>(jsonOptions);
            Assert.NotEqual(Guid.Empty, createdId);
            Assert.Equal(newUser.Id, createdId);
        }

        [Fact]
        public async Task UpsertUser_Updates_Existing_User()
        {
            var existingUserId = Guid.NewGuid();
            var updatedUser = new UserDetailModel
            {
                Id = existingUserId,
                Name = "Updated Existing User",
                PhotoUrl = "http://example.com/updated_photo.jpg",
                Role = Role.User
            };

            var response = await client.Value.PostAsJsonAsync("/api/users/upsert", updatedUser, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updatedId = await response.Content.ReadFromJsonAsync<Guid>(jsonOptions);
            Assert.Equal(existingUserId, updatedId);

            var getResponse = await client.Value.GetAsync($"/api/users/{existingUserId}");
            var user = await getResponse.Content.ReadFromJsonAsync<UserDetailModel>(jsonOptions);
            Assert.Equal(updatedUser.Name, user!.Name);
            Assert.Equal(updatedUser.PhotoUrl, user.PhotoUrl);
        }

        [Fact]
        public async Task DeleteUser_Removes_User()
        {
            var userId = Guid.NewGuid();

            var response = await client.Value.DeleteAsync($"/api/users/{userId}");

            response.EnsureSuccessStatusCode();
            var getResponse = await client.Value.GetAsync($"/api/users/{userId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        public async ValueTask DisposeAsync()
        {
            await application.DisposeAsync();
        }
    }
}
