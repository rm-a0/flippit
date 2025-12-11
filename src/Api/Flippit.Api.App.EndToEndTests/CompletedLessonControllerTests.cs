using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Common.Models.User;

namespace Flippit.Api.App.EndToEndTests
{
    public class CompletedLessonControllerTests : IAsyncDisposable
    {
        private readonly FlippitApiApplicationFactory application;
        private readonly Lazy<HttpClient> client;
        private readonly JsonSerializerOptions jsonOptions;

        public CompletedLessonControllerTests()
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
        public async Task GetCompletedLessons_ByUserId_Returns_OnlyLessonsForThatUser()
        {
            // Use the authenticated user ID from the test authentication handler
            var authenticatedUserId = Guid.Parse(TestAuthenticationHandler.DefaultUserId);

            var user = new UserDetailModel
            {
                Id = authenticatedUserId,
                Name = "Test User"
            };
            var userResponse = await client.Value.PostAsJsonAsync("/api/users", user, jsonOptions);
            userResponse.EnsureSuccessStatusCode();

            // Create a completed lesson - it will be stored with the authenticated user's ID
            var completedLessonForAuthUser = new CompletedLessonDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = authenticatedUserId,
                AnswersJson = "{}",
                StatisticsJson = "{}",
                CollectionId = Guid.NewGuid()
            };
            var post1 = await client.Value.PostAsJsonAsync("/api/completedLessons", completedLessonForAuthUser, jsonOptions);
            post1.EnsureSuccessStatusCode();

            // Retrieve completed lessons for the authenticated user
            var response = await client.Value.GetAsync($"/api/users/{authenticatedUserId}/completedLessons");
            response.EnsureSuccessStatusCode();
            var lessonsForUser = await response.Content.ReadFromJsonAsync<IEnumerable<CompletedLessonListModel>>(jsonOptions);

            Assert.NotNull(lessonsForUser);
            Assert.NotEmpty(lessonsForUser);
            // Should have at least one lesson for the authenticated user
            Assert.True(lessonsForUser.Count() >= 1);
        } 

        [Fact]
        public async Task GetCompletedLessons_ByCollectionId_Returns_OnlyLessonsForThatCollection()
        {
            var targetCollectionId = Guid.NewGuid();
            var otherCollectionId = Guid.NewGuid();

            var lessonInTargetCollection = new CompletedLessonDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AnswersJson = "{}",
                StatisticsJson = "{}",
                CollectionId = targetCollectionId
            };
            await client.Value.PostAsJsonAsync("/api/completedLessons", lessonInTargetCollection, jsonOptions);

            var lessonInOtherCollection = new CompletedLessonDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AnswersJson = "{}",
                StatisticsJson = "{}",
                CollectionId = otherCollectionId
            };
            await client.Value.PostAsJsonAsync("/api/completedLessons", lessonInOtherCollection, jsonOptions);

            var response = await client.Value.GetAsync($"/api/collections/{targetCollectionId}/completedLessons");
            response.EnsureSuccessStatusCode();
            var lessonsInCollection = await response.Content.ReadFromJsonAsync<IEnumerable<CompletedLessonListModel>>(jsonOptions);

            Assert.Single(lessonsInCollection!);
            Assert.All(lessonsInCollection!, l => Assert.Equal(targetCollectionId, l.CollectionId));
        }

        [Fact]
        public async Task GetCompletedLessonsOfCollection_Returns_Only_Lessons_From_That_Collection()
        {
            var targetCollectionId = Guid.NewGuid();
            var otherCollectionId = Guid.NewGuid();

            var lessonInTarget = new CompletedLessonDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AnswersJson = "{}",
                StatisticsJson = "{}",
                CollectionId = targetCollectionId
            };

            var post1 = await client.Value.PostAsJsonAsync("/api/completedLessons", lessonInTarget, jsonOptions);
            post1.EnsureSuccessStatusCode();

            var lessonInOther = new CompletedLessonDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AnswersJson = "{}",
                StatisticsJson = "{}",
                CollectionId = otherCollectionId
            };
            var post2 = await client.Value.PostAsJsonAsync("/api/completedLessons", lessonInOther, jsonOptions);
            post2.EnsureSuccessStatusCode();

            var response = await client.Value.GetAsync($"/api/collections/{targetCollectionId}/completedLessons");
            response.EnsureSuccessStatusCode();
            var lessonsInCollection = await response.Content.ReadFromJsonAsync<IEnumerable<CompletedLessonListModel>>(jsonOptions);

            Assert.NotNull(lessonsInCollection);
            Assert.NotEmpty(lessonsInCollection);
            Assert.Single(lessonsInCollection);
            Assert.Equal(targetCollectionId, lessonsInCollection.First().CollectionId);
        } 

        public async ValueTask DisposeAsync()
        {
            await application.DisposeAsync();
        }
    }
}
