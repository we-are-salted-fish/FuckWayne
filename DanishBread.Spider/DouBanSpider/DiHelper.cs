using System;
using System.IO;
using AspectCore.Extensions.DependencyInjection;
using FuckWayne.Configuration;
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
            //日志
            var basePath = AppContext.BaseDirectory;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(basePath,"logs", "log-.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
            //配置
            var configuration = Configuration.Build();
            // 构建容器
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, SerilogLoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddHttpClient();
            services.AddSingleton(configuration);

            var serviceProvider = services.BuildDynamicProxyProvider();
            var logger = serviceProvider.GetService<ILogger<DiHelper>>();
            logger.LogInformation("DI 创建成功");
            
            return serviceProvider;
        }

        
    }
}
