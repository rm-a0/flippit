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
            Assert.Equal(QAType.text, card.QuestionType);
            Assert.Equal(QAType.text, card.AnswerType);
            Assert.Equal("Basic geography question", card.Description);
        }

        [Fact]
        public async Task GetCardById_NonExistentId_Returns_NotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var response = await client.Value.GetAsync($"/api/cards/{nonExistentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // [Fact]
        // public async Task CreateCard_Returns_Created_Card()
        // {
        //     var newCard = new CardDetailModel
        //     {
        //         Id = Guid.NewGuid(),
        //         QuestionType = QAType.text,
        //         AnswerType = QAType.text,
        //         Question = "What is the capital of Japan?",
        //         Answer = "Tokyo",
        //         Description = "Geography test",
        //         CreatorId = Guid.NewGuid(),
        //         CollectionId = Guid.NewGuid()
        //     };

        //     var response = await client.Value.PostAsJsonAsync("/api/cards", newCard, jsonOptions);

        //     Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //     var createdCard = await response.Content.ReadFromJsonAsync<CardDetailModel>(jsonOptions);
        //     Assert.NotNull(createdCard);
        //     Assert.Equal(newCard.Id, createdCard.Id);
        //     Assert.Equal(newCard.Question, createdCard.Question);
        //     Assert.Equal(newCard.Answer, createdCard.Answer);
        //     Assert.Equal(newCard.QuestionType, createdCard.QuestionType);
        //     Assert.Equal(newCard.AnswerType, createdCard.AnswerType);
        //     Assert.Equal(newCard.Description, createdCard.Description);
        // }

        // [Fact]
        // public async Task UpdateCard_Returns_Updated_Card()
        // {
        //     var cardId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-1234567890ab");
        //     var updatedCard = new CardDetailModel
        //     {
        //         Id = cardId,
        //         QuestionType = QAType.text,
        //         AnswerType = QAType.text,
        //         Question = "What is the capital of France? (Updated)",
        //         Answer = "Paris, France",
        //         Description = "Updated geography question",
        //         CreatorId = Guid.Parse("d4e5f6a7-b890-1234-defa-4567890123de"),
        //         CollectionId = Guid.Parse("f6a7b890-1234-5678-fabc-6789012345fa")
        //     };

        //     var response = await client.Value.PutAsJsonAsync($"/api/cards/{cardId}", updatedCard, jsonOptions);

        //     response.EnsureSuccessStatusCode();
        //     var returnedCard = await response.Content.ReadFromJsonAsync<CardDetailModel>(jsonOptions);
        //     Assert.NotNull(returnedCard);
        //     Assert.Equal(updatedCard.Question, returnedCard.Question);
        //     Assert.Equal(updatedCard.Answer, returnedCard.Answer);
        //     Assert.Equal(updatedCard.Description, returnedCard.Description);
        // }

        // [Fact]
        // public async Task UpdateCard_NonExistentId_Returns_NotFound()
        // {
        //     var nonExistentId = Guid.NewGuid();
        //     var updatedCard = new CardDetailModel
        //     {
        //         Id = nonExistentId,
        //         QuestionType = QAType.text,
        //         AnswerType = QAType.text,
        //         Question = "Test",
        //         Answer = "Test",
        //         Description = "Test",
        //         CreatorId = Guid.NewGuid(),
        //         CollectionId = Guid.NewGuid()
        //     };

        //     var response = await client.Value.PutAsJsonAsync($"/api/cards/{nonExistentId}", updatedCard, jsonOptions);

        //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        // }

        [Fact]
        public async Task DeleteCard_Removes_Card()
        {
            var cardId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-2345678901bc");

            var response = await client.Value.DeleteAsync($"/api/cards/{cardId}");

            response.EnsureSuccessStatusCode();

            var getResponse = await client.Value.GetAsync($"/api/cards/{cardId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        // [Fact]
        // public async Task DeleteCard_NonExistentId_Returns_NotFound()
        // {
        //     var nonExistentId = Guid.NewGuid();

        //     var response = await client.Value.DeleteAsync($"/api/cards/{nonExistentId}");

        //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        // }

        [Fact]
        public async Task GetAllCards_WithFilter_Returns_Filtered_Cards()
        {
            var response = await client.Value.GetAsync("/api/cards?filter=France");

            response.EnsureSuccessStatusCode();
            var cards = await response.Content.ReadFromJsonAsync<ICollection<CardListModel>>(jsonOptions);
            Assert.NotNull(cards);
            Assert.NotEmpty(cards);
            Assert.All(cards, card => Assert.Contains("France", card.Question, StringComparison.OrdinalIgnoreCase));
        }

        public async ValueTask DisposeAsync()
        {
            await application.DisposeAsync();
        }
    }
}
