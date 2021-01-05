using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DouBanSpider
{
    public class DouBanClient
    {
        private readonly HttpClient _httpClient;
        
        public DouBanClient(HttpClient client)
        {
            _httpClient = client;
        }

        
        public async Task<string> GetStringAsync(string url, Encoding encoding = null)
        {
            encoding ??= Encoding.GetEncoding("UTF-8");
            var uri = new Uri(url);
            using var response = await _httpClient.GetAsync(uri);
            var source = encoding.GetString(await response.Content.ReadAsByteArrayAsync());
            return source;
        }

        public async Task<Stream> GetStreamAsync(string url)
        {
            var uri = new Uri(url);
            var source = await _httpClient.GetStreamAsync(uri);
            return source;
        }
    }
}