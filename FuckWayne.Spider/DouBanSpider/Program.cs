using FuckWayne.Configuration;
using FuckWayne.DouBan;
using System;
using System.IO;

namespace DouBanSpider
{
    class Program
    {

        static Configuration config;

        static void Main(string[] args)
        {
            Console.Title = "FuckWayne!";            
            Console.WriteLine("welcome FuckWayne!");

            config = Configuration.Build();

            if (!Directory.Exists(config.SaveFolder))
            {
                Directory.CreateDirectory(config.SaveFolder);
            }

            NewLife.Log.XTrace.UseConsole();
            var wayne = new DouBan(config.SaveFolder);
            wayne.DownloadAllImage();

            Console.ReadKey();
        }
    }
}
