namespace Flippit.Web.BL
{
    // Partial class extensions to configure API clients properly
    public partial class AuthApiClient
    {
        partial void Initialize()
        {
            // Use the HttpClient's BaseAddress if it's configured
            if (_httpClient.BaseAddress != null)
            {
                BaseUrl = _httpClient.BaseAddress.ToString().TrimEnd('/');
            }
        }
    }

    public partial class ApiApiClient
    {
        partial void Initialize()
        {
            // Use the HttpClient's BaseAddress if it's configured
            if (_httpClient.BaseAddress != null)
            {
                BaseUrl = _httpClient.BaseAddress.ToString().TrimEnd('/');
            }
        }
    }

    public partial class UsersApiClient
    {
        partial void Initialize()
        {
            // Use the HttpClient's BaseAddress if it's configured
            if (_httpClient.BaseAddress != null)
            {
                BaseUrl = _httpClient.BaseAddress.ToString().TrimEnd('/');
            }
        }
    }

    public partial class CardsApiClient
    {
        partial void Initialize()
        {
            // Use the HttpClient's BaseAddress if it's configured
            if (_httpClient.BaseAddress != null)
            {
                BaseUrl = _httpClient.BaseAddress.ToString().TrimEnd('/');
            }
        }
    }

    public partial class CollectionsApiClient
    {
        partial void Initialize()
        {
            // Use the HttpClient's BaseAddress if it's configured
            if (_httpClient.BaseAddress != null)
            {
                BaseUrl = _httpClient.BaseAddress.ToString().TrimEnd('/');
            }
        }
    }

    public partial class CompletedLessonsApiClient
    {
        partial void Initialize()
        {
            // Use the HttpClient's BaseAddress if it's configured
            if (_httpClient.BaseAddress != null)
            {
                BaseUrl = _httpClient.BaseAddress.ToString().TrimEnd('/');
            }
        }
    }
}
