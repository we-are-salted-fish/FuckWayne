using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DouBanSpider
{
    public class HttpHelper
    {
        //static readonly HttpClient http = new HttpClient();

        public static async Task<string> GetStringAsync(string url, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("UTF-8");
            }

            var source = string.Empty;

            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            using (var http = new HttpClient(handler))
            {
                var uri = new Uri(url);

                http.DefaultRequestHeaders.UserAgent.Clear();
                http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.27 Safari/537.36 OPR/26.0.1656.8 (Edition beta)");

                var response = await http.GetAsync(uri);
                source = encoding.GetString(await response.Content.ReadAsByteArrayAsync());
                handler.Dispose();
            }

            return source;
        }

        public static async Task<Stream> GetStreamAsync(string url)
        {
            Stream source = null;

            using (var http = new HttpClient())
            {
                var uri = new Uri(url);

                http.DefaultRequestHeaders.UserAgent.Clear();
                http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.27 Safari/537.36 OPR/26.0.1656.8 (Edition beta)");

                source = await http.GetStreamAsync(uri);
            }

            return source;
        }
    }
}
