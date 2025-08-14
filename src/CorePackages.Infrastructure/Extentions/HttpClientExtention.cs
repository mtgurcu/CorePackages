using Newtonsoft.Json;
using System.Text;

namespace CorePackages.Infrastructure.Extentions
{
    public static class HttpClientExtention
    {
        public static async Task<T> SendAsync<T>(this HttpClient _httpClient, HttpMethod method, string url, object content)
        {
            var request = new HttpRequestMessage(method, url);

            if (content != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (typeof(T) == typeof(string))
                return (T)(object)await response.Content.ReadAsStringAsync();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString)!;
        }
        public static async Task<T> SendAsync<T>(this HttpClient _httpClient, HttpMethod method, string url, HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (typeof(T) == typeof(string))
                return (T)(object)await response.Content.ReadAsStringAsync();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString)!;
        }
        public static async Task<T> SendAsync<T>(this HttpClient _httpClient, HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (typeof(T) == typeof(string))
                return (T)(object)await response.Content.ReadAsStringAsync();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString)!;
        }
        public static async Task SendAsync(this HttpClient _httpClient, HttpMethod method, string url, object? content = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (content != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
