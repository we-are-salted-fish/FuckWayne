using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DouBanSpider
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "good good study,day day up!";            
            Console.WriteLine("我希望有个如你一般的人，如山间清爽的风，如古城温暖的光，从清晨到夜晚，从山野到书房，等待不怕岁月蹉跎，不怕路途遥远，只要最后是你就好。");

            var di = DiHelper.BuildDi();
            var options = di.GetService<IOptions<DouBanOptions>>();
            
            var saveFolder = options?.Value.SaveFolder;
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            var wayne = di.GetService<DouBan>();
            wayne.DownloadAllImage();
            
            Console.ReadKey();
        }
    }
}
