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

            Console.Title = "good good study,day day up!";            
            Console.WriteLine("sry...");

            var logger = di.GetService<ILogger<Program>>();
            logger.LogInformation("123");
            
            var config = di.GetService<FuckWayne.Configuration.Configuration>();
            
            if (!Directory.Exists(config.SaveFolder))
            {
                Directory.CreateDirectory(config.SaveFolder);
            }

            NewLife.Log.XTrace.UseConsole();
            var wayne = new DouBan(config.SaveFolder, config.Categorys);
            wayne.DownloadAllImage();

            Console.ReadKey();
        }
    }
}
