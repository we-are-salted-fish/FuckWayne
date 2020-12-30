using FuckWayne.Configuration;
using FuckWayne.DouBan;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DouBanSpider
{
    class Program
    {


        static void Main(string[] args)
        {
            var di = DiHelper.BuildDi();

            Console.Title = "FuckWayne!";            
            Console.WriteLine("welcome FuckWayne!");

            var logger = di.GetService<ILogger<Program>>();
            logger.LogInformation("123");
            
            var config = di.GetService<FuckWayne.Configuration.Configuration>();
            
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
