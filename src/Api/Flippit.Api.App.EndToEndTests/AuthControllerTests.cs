using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Flippit.Api.App.Models;
using Flippit.Common.Enums;
using Xunit;

namespace Flippit.Api.App.EndToEndTests
{
    public class AuthControllerTests : IAsyncDisposable
    {
        private readonly FlippitApiApplicationFactory application;
        private readonly Lazy<HttpClient> client;
        private readonly JsonSerializerOptions jsonOptions;

        public AuthControllerTests()
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
        public async Task Register_WithValidCredentials_Returns_Success()
        {
            var registerRequest = new RegisterRequest
            {
                UserName = $"testuser_{Guid.NewGuid()}",
                Password = "Test123!",
                Email = "test@example.com"
            };

            var response = await client.Value.PostAsJsonAsync("/api/auth/register", registerRequest, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>(jsonOptions);
            Assert.NotNull(registerResponse);
            Assert.NotEqual(Guid.Empty, registerResponse.UserId);
            Assert.Equal(registerRequest.UserName, registerResponse.UserName);
        }

        [Fact]
        public async Task Register_WithExistingUser_Returns_BadRequest()
        {
            var userName = $"existinguser_{Guid.NewGuid()}";
            var registerRequest = new RegisterRequest
            {
                UserName = userName,
                Password = "Test123!",
                Email = "test@example.com"
            };

            // First registration should succeed
            var firstResponse = await client.Value.PostAsJsonAsync("/api/auth/register", registerRequest, jsonOptions);
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

            // Second registration with same username should fail
            var secondResponse = await client.Value.PostAsJsonAsync("/api/auth/register", registerRequest, jsonOptions);
            Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
        }

        [Fact]
        public async Task Register_WithMissingUsername_Returns_BadRequest()
        {
            var registerRequest = new
            {
                UserName = "",
                Password = "Test123!",
                Email = "test@example.com"
            };

            var response = await client.Value.PostAsJsonAsync("/api/auth/register", registerRequest, jsonOptions);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithMissingPassword_Returns_BadRequest()
        {
            var registerRequest = new
            {
                UserName = "testuser",
                Password = "",
                Email = "test@example.com"
            };

            var response = await client.Value.PostAsJsonAsync("/api/auth/register", registerRequest, jsonOptions);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_AfterRegistration_Returns_Token()
        {
            // Register a new user
            var userName = $"loginuser_{Guid.NewGuid()}";
            var password = "Test123!";
            var registerRequest = new RegisterRequest
            {
                UserName = userName,
                Password = password,
                Email = "login@example.com"
            };

            var registerResponse = await client.Value.PostAsJsonAsync("/api/auth/register", registerRequest, jsonOptions);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            // Now login with the same credentials
            var loginRequest = new LoginRequest
            {
                UserName = userName,
                Password = password
            };

            var loginResponse = await client.Value.PostAsJsonAsync("/api/auth/login", loginRequest, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(jsonOptions);
            Assert.NotNull(loginResult);
            Assert.NotEmpty(loginResult.Token);
            Assert.Equal(userName, loginResult.UserName);
            Assert.Contains("User", loginResult.Roles);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_Returns_Unauthorized()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "nonexistentuser",
                Password = "WrongPassword123!"
            };

            var response = await client.Value.PostAsJsonAsync("/api/auth/login", loginRequest, jsonOptions);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AuthenticatedEndpoint_WithValidToken_Returns_Success()
        {
            // In Testing environment, the test authentication handler provides a valid user
            // so we can test that protected endpoints work
            var userName = $"authuser_{Guid.NewGuid()}";
            var password = "Test123!";

            // Register a user
            var registerRequest = new RegisterRequest
            {
                UserName = userName,
                Password = password,
                Email = "auth@example.com"
            };
            await client.Value.PostAsJsonAsync("/api/auth/register", registerRequest, jsonOptions);

            // Login to get a token
            var loginRequest = new LoginRequest
            {
                UserName = userName,
                Password = password
            };
            var loginResponse = await client.Value.PostAsJsonAsync("/api/auth/login", loginRequest, jsonOptions);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(jsonOptions);

            // Note: In the Testing environment, the TestAuthenticationHandler is used instead of JWT

            // Try to create a user (requires authentication)
            var newUser = new Flippit.Common.Models.User.UserDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                PhotoUrl = "http://example.com/photo.jpg",
                Role = Role.User
            };

            var response = await client.Value.PostAsJsonAsync("/api/users", newUser, jsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public async ValueTask DisposeAsync()
        {
            await application.DisposeAsync();
        }
    }
}
