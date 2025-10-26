using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Common.Models.User;

namespace Flippit.Api.App.EndToEndTests
{
    public class CompletedLessonControllerTests : IAsyncDisposable
    {
        private readonly FlippitApiApplicationFactory application;
        private readonly Lazy<HttpClient> client;

        public CompletedLessonControllerTests()
        {
            application = new FlippitApiApplicationFactory();
            client = new Lazy<HttpClient>(application.CreateClient());
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
        public async Task GetCompletedLessonsOfCollection_return_only_one()
        {
            var collectionId = Guid.NewGuid();

            var CompletedLesson1 = new CompletedLessonDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AnswersJson = "{}",
                StatisticsJson = "{}",
                CollectionId = collectionId,

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

            var response = await client.Value.GetAsync($"api/collections/{collectionId}/completedLessons");
            response.EnsureSuccessStatusCode();

            var userCompletedLessons = await response.Content.ReadFromJsonAsync<IEnumerable<CompletedLessonListModel>>();
            Assert.NotEmpty(userCompletedLessons!);
            Assert.Single(userCompletedLessons!);
        }
        public async ValueTask DisposeAsync()
        {
            await application.DisposeAsync();
        }
    }
}
