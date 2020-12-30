using Microsoft.Extensions.Configuration;
using System.IO;

namespace FuckWayne.Configuration
{
    public class Configuration
    {
        public string SaveFolder { get; set; }
       

        public static Configuration Build()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            return new Configuration
            {
                SaveFolder = config[nameof(SaveFolder)]
            };
        }
    }
}
