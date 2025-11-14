using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Flippit.Common.Models.Card;
using Flippit.Common.Models.Collection;

namespace Flippit.Api.App.EndToEndTests
{
    public class CollectionControllerTests : IAsyncDisposable
    {
        private readonly FlippitApiApplicationFactory application;
        private readonly Lazy<HttpClient> client;
        private readonly JsonSerializerOptions jsonOptions;

        public CollectionControllerTests()
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
        public async Task GetAllCollections()
        {
            var response = await client.Value.GetAsync("/api/Collections");

            response.EnsureSuccessStatusCode();

            var collections = await response.Content.ReadFromJsonAsync<ICollection<CollectionListModel>>();
            Assert.NotNull(collections);
            Assert.NotEmpty(collections);
        }

        [Fact]
        public async Task CreateCollection()
        {
            var guidId = Guid.NewGuid();
            var newCollection = new CollectionDetailModel { Id = guidId, CreatorId = Guid.Parse("d4e5f6a7-b890-1234-defa-4567890123de"), StartTime = DateTime.Now, EndTime = DateTime.Now, Name ="TestCollection" }; 
            var response = await client.Value.PostAsJsonAsync("/api/Collections", newCollection);

            response.EnsureSuccessStatusCode();

            var responseGuid = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.Equal(guidId, responseGuid);
        }

        [Fact]
        public async Task CreateCollectionAddCards()
        {
            var collectionGuid = Guid.NewGuid();
            var newCollection = new CollectionDetailModel { Id = collectionGuid, CreatorId = Guid.Parse("d4e5f6a7-b890-1234-defa-4567890123de"), StartTime = DateTime.Now, EndTime = DateTime.Now, Name = "TestCollection" };
            var collectionResponse = await client.Value.PostAsJsonAsync("/api/collections", newCollection);
            collectionResponse.EnsureSuccessStatusCode();

            var firstCard = new CardDetailModel
            {
                Id = Guid.NewGuid(),
                QuestionType = Common.Enums.QAType.Text,
                AnswerType = Common.Enums.QAType.Text,
                Question = "TestQuestion",
                Answer = "TestAnswer",
                Description = "Test Card",
                CreatorId = Guid.Parse("d4e5f6a7-b890-1234-defa-4567890123de"),
                CollectionId = collectionGuid
            };

            var secondCard = new CardDetailModel
            {
                Id = Guid.NewGuid(),
                QuestionType = Common.Enums.QAType.Text,
                AnswerType = Common.Enums.QAType.Text,
                Question = "TestQuestion",
                Answer = "TestAnswer",
                Description = "Test Card",
                CreatorId = Guid.Parse("d4e5f6a7-b890-1234-defa-4567890123de"),
                CollectionId = collectionGuid
            };

            var json = JsonSerializer.Serialize(firstCard, jsonOptions);


            var card1Response = await client.Value.PostAsJsonAsync("/api/cards", firstCard, jsonOptions);
            card1Response.EnsureSuccessStatusCode();

            var card2Response = await client.Value.PostAsJsonAsync("/api/cards", firstCard, jsonOptions);
            card2Response.EnsureSuccessStatusCode();

            var response = await client.Value.GetAsync($"/api/collections/{collectionGuid}/cards");

            response.EnsureSuccessStatusCode();

            var cards = await response.Content.ReadFromJsonAsync<ICollection<CardListModel>>(jsonOptions);
            Assert.NotEmpty(cards!);
            Assert.Equal(2, cards!.Count);
        }

        public async ValueTask DisposeAsync()
        {
            await application.DisposeAsync();
        }
    }
}
