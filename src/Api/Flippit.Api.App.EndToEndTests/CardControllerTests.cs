using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Flippit.Common.Enums;
using Flippit.Common.Models.Card;
using Xunit;

namespace Flippit.Api.App.EndToEndTests
{
    public class CardControllerTests : IAsyncDisposable
    {
        private readonly FlippitApiApplicationFactory application;
        private readonly Lazy<HttpClient> client;
        private readonly JsonSerializerOptions jsonOptions;

        public CardControllerTests()
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
        public async Task GetAllCards_Returns_At_Least_One_Card()
        {
            var response = await client.Value.GetAsync("/api/cards");

            response.EnsureSuccessStatusCode();
            var cards = await response.Content.ReadFromJsonAsync<ICollection<CardListModel>>(jsonOptions);
            Assert.NotNull(cards);
            Assert.NotEmpty(cards);
        }

        [Fact]
        public async Task GetCardById_Returns_Correct_Card()
        {
            var cardId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-1234567890ab");

            var response = await client.Value.GetAsync($"/api/cards/{cardId}");

            response.EnsureSuccessStatusCode();
            var card = await response.Content.ReadFromJsonAsync<CardDetailModel>(jsonOptions);
            Assert.NotNull(card);
            Assert.Equal(cardId, card.Id);
            Assert.Equal("What is the capital of France?", card.Question);
            Assert.Equal("Paris", card.Answer);
            Assert.Equal(QAType.Text, card.QuestionType);
            Assert.Equal(QAType.Text, card.AnswerType);
            Assert.Equal("Basic geography question", card.Description);
        }

        [Fact]
        public async Task GetCardById_NonExistentId_Returns_NotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var response = await client.Value.GetAsync($"/api/cards/{nonExistentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateCard_Returns_Created_Card()
        {
            var newCard = new CardDetailModel
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the capital of Japan?",
                Answer = "Tokyo",
                Description = "Geography test",
                CreatorId = Guid.NewGuid(),
                CollectionId = Guid.NewGuid()
            };

            var response = await client.Value.PostAsJsonAsync("/api/cards", newCard, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdCardId = await response.Content.ReadFromJsonAsync<Guid>(jsonOptions);
            Assert.NotEqual(Guid.Empty, createdCardId);
            Assert.Equal(newCard.Id, createdCardId);
        }

        [Fact]
        public async Task DeleteCard_Removes_Card()
        {
            var cardId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-2345678901bc");

            var response = await client.Value.DeleteAsync($"/api/cards/{cardId}");

            response.EnsureSuccessStatusCode();

            var getResponse = await client.Value.GetAsync($"/api/cards/{cardId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        public async ValueTask DisposeAsync()
        {
            await application.DisposeAsync();
        }

        [Fact]
        public async Task UpdateCard_Returns_Updated_Card_Id()
        {
            var cardId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-1234567890ab");
            var updatedCard = new CardDetailModel
            {
                Id = cardId,
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the capital of Brazil?",
                Answer = "Brasilia",
                Description = "Updated geography question",
                CreatorId = Guid.NewGuid(),
                CollectionId = Guid.NewGuid()
            };

            var response = await client.Value.PutAsJsonAsync("/api/cards", updatedCard, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var returnedId = await response.Content.ReadFromJsonAsync<Guid?>(jsonOptions);
            Assert.Equal(cardId, returnedId);
        }

        [Fact]
        public async Task UpsertCard_Creates_New_Card()
        {
            var newCard = new CardDetailModel
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the capital of Canada?",
                Answer = "Ottawa",
                Description = "Geography test",
                CreatorId = Guid.NewGuid(),
                CollectionId = Guid.NewGuid()
            };

            var response = await client.Value.PostAsJsonAsync("/api/cards/upsert", newCard, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var createdId = await response.Content.ReadFromJsonAsync<Guid>(jsonOptions);
            Assert.NotEqual(Guid.Empty, createdId);
            Assert.Equal(newCard.Id, createdId);
        }

        [Fact]
        public async Task GetAllCards_WithFilter_Returns_Filtered_Cards()
        {
            var filter = "France";

            var response = await client.Value.GetAsync($"/api/cards?filter={filter}");

            response.EnsureSuccessStatusCode();
            var cards = await response.Content.ReadFromJsonAsync<ICollection<CardListModel>>(jsonOptions);
            Assert.NotNull(cards);
            Assert.All(cards, card => Assert.Contains(filter, card.Question, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetAllCards_WithSortByQuestion_Returns_Sorted_Cards()
        {
            var response = await client.Value.GetAsync("/api/cards?sortBy=question");

            response.EnsureSuccessStatusCode();
            var cards = await response.Content.ReadFromJsonAsync<ICollection<CardListModel>>(jsonOptions);
            Assert.NotNull(cards);
            Assert.NotEmpty(cards);
            var sortedQuestions = cards.Select(c => c.Question).ToList();
            var expectedOrder = sortedQuestions.OrderBy(q => q).ToList();
            Assert.Equal(expectedOrder, sortedQuestions);
        }

        [Fact]
        public async Task GetAllCards_WithPagination_Returns_Paged_Result()
        {
            var page = 1;
            var pageSize = 2;

            var response = await client.Value.GetAsync($"/api/cards?page={page}&pageSize={pageSize}");

            response.EnsureSuccessStatusCode();
            var cards = await response.Content.ReadFromJsonAsync<ICollection<CardListModel>>(jsonOptions);
            Assert.NotNull(cards);
            Assert.True(cards.Count <= pageSize, "Returned cards exceed page size");
        }

        [Fact]
        public async Task UpsertCard_Updates_Existing_Card()
        {
            var existingCardId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-1234567890ab");
            var updatedCard = new CardDetailModel
            {
                Id = existingCardId,
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the capital of Italy?",
                Answer = "Rome",
                Description = "Updated geography test",
                CreatorId = Guid.NewGuid(),
                CollectionId = Guid.NewGuid()
            };

            var response = await client.Value.PostAsJsonAsync("/api/cards/upsert", updatedCard, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updatedId = await response.Content.ReadFromJsonAsync<Guid>(jsonOptions);
            Assert.Equal(existingCardId, updatedId);

            var getResponse = await client.Value.GetAsync($"/api/cards/{existingCardId}");
            var card = await getResponse.Content.ReadFromJsonAsync<CardDetailModel>(jsonOptions);
            Assert.Equal(updatedCard.Question, card!.Question);
            Assert.Equal(updatedCard.Answer, card!.Answer);
        }

        [Fact]
        public async Task UpdateCard_NonExistentId_Returns_Ok_With_Null()
        {
            var nonExistentId = Guid.NewGuid();
            var updatedCard = new CardDetailModel
            {
                Id = nonExistentId,
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the capital of Brazil?",
                Answer = "Brasilia",
                Description = "Geography question",
                CreatorId = Guid.NewGuid(),
                CollectionId = Guid.NewGuid()
            };

            var response = await client.Value.PutAsJsonAsync("/api/cards", updatedCard, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(string.IsNullOrEmpty(content) || content == "null", "Expected empty or null response body");
        }
    }
}
