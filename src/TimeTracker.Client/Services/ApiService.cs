using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TimeTracker.Client.Security;

namespace TimeTracker.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenAuthenticationStateProvider _authStateProvider;

        public ApiService(HttpClient httpClient, TokenAuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<T> GetAsync<T>(string url, string token = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                token = await _authStateProvider.GetTokenAsync();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{Config.ApiResourceUrl}{url}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsByteArrayAsync();
            return JsonSerializer.Parse<T>(
                responseContent, 
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
    }
}
