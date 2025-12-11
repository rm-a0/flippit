using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace Flippit.Web.App.Services
{
    public class AuthenticationService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;
        private const string TokenKey = "authToken";
        private const string UserIdKey = "userId";

        public AuthenticationService(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            }
            catch (JSException)
            {
                // localStorage not available (e.g., during prerendering)
                return null;
            }
        }

        public async Task SetTokenAsync(string token, Guid userId)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserIdKey, userId.ToString());
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            catch (JSException)
            {
                // localStorage not available (e.g., during prerendering)
            }
        }

        public async Task<Guid?> GetUserIdAsync()
        {
            try
            {
                var userIdStr = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserIdKey);
                if (Guid.TryParse(userIdStr, out var userId))
                {
                    return userId;
                }
            }
            catch (JSException)
            {
                // localStorage not available (e.g., during prerendering)
            }
            return null;
        }

        public async Task ClearTokenAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserIdKey);
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            catch (JSException)
            {
                // localStorage not available (e.g., during prerendering)
            }
        }

        public async Task RestoreAuthenticationAsync()
        {
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
