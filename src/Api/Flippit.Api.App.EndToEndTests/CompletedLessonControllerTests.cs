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
        public async Task GetCompletedLessonsOfUser_return_only_one()
        {
            var userId = Guid.NewGuid();
            var user = new UserDetailModel
            {
                Id = userId,
                Name = "Test"
            };

            var CompletedLesson1 = new CompletedLessonDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AnswersJson = "{}",
                StatisticsJson = "{}",
                CollectionId = Guid.NewGuid(),

            };

            var lesson1response = await client.Value.PostAsJsonAsync("/api/completedLessons", CompletedLesson1);
            lesson1response.EnsureSuccessStatusCode();

            var CompletedLesson2 = new CompletedLessonDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AnswersJson = "{}",
                StatisticsJson = "{}",
                CollectionId = Guid.NewGuid(),

            };

            var lesson2response = await client.Value.PostAsJsonAsync("/api/completedLessons", CompletedLesson2);
            lesson2response.EnsureSuccessStatusCode();

            var response = await client.Value.GetAsync($"api/users/{userId}/completedLessons");
            response.EnsureSuccessStatusCode();

            var userCompletedLessons = await response.Content.ReadFromJsonAsync<IEnumerable<CompletedLessonListModel>>();
            Assert.NotEmpty(userCompletedLessons!);
            Assert.Single(userCompletedLessons!);
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
