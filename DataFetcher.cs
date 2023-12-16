using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDataCollector
{
    public class DataFetcher
    {
        private readonly HttpClient _httpClient;

        public DataFetcher()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> FetchDataFromApiAsync(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error fetching data: {e.Message}");
                throw;
            }
        }
    }
}
