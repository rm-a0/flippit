using System;
using System.Collections.Generic;
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

        public CardControllerTests()
        {
            application = new FlippitApiApplicationFactory();
            client = new Lazy<HttpClient>(application.CreateClient());
        }

        [Fact]
        public async Task GetAllCards_Returns_At_Least_One_Card()
        {
            var response = await client.Value.GetAsync("/api/cards");

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
            };
            var cards = await response.Content.ReadFromJsonAsync<ICollection<CardListModel>>(options);
            Assert.NotNull(cards);
            Assert.NotEmpty(cards);
        }

        public async ValueTask DisposeAsync()
        {
            await application.DisposeAsync();
        }
    }
}
