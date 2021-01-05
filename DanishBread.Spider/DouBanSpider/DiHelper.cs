using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace DouBanSpider
{
    public class DiHelper
    {
        public static IServiceProvider BuildDi()
        {
            var configuration = GetConfiguration();
            //日志
            var basePath = AppContext.BaseDirectory;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(basePath,"logs", "log-.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
            // 构建容器
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, SerilogLoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddSingleton<DouBan>();
            //豆瓣配置
            services.Configure<DouBanOptions>(options =>
            {
                options.Categories = configuration.GetSection("Categories").Get<List<int>>();
                options.SaveFolder = configuration.GetSection("SaveFolder").Value;
            });
            
            services.AddHttpClient<DouBanClient>()
                .ConfigureHttpClient(http =>
                {
                    http.DefaultRequestHeaders.UserAgent.Clear();
                    http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.27 Safari/537.36 OPR/26.0.1656.8 (Edition beta)");
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                });

            var serviceProvider = services.BuildDynamicProxyProvider();
            var logger = serviceProvider.GetService<ILogger<DiHelper>>();
            logger.LogInformation("DI 创建成功");
            
            return serviceProvider;
        }
        
        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }
        
    }
}
