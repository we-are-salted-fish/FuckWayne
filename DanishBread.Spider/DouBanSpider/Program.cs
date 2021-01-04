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
            Console.WriteLine("我希望有个如你一般的人，如山间清爽的风，如古城温暖的光，从清晨到夜晚，从山野到书房，等待不怕岁月蹉跎，不怕路途遥远，只要最后是你就好。");

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
