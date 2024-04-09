using Autofac.Extensions.DependencyInjection;
using Destructurama;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using System;
using Tasker.Infrastructure;

namespace Tasker
{
    class Program
    {
        static void Main(string[] args)
        {
            IHost Host = CreateHostBuilder(args).Build();
            Host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            })
            .UseSerilog((ctx, ConfigurationBinder) =>
            {
                ConfigurationBinder.MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Destructure.JsonNetTypes()
                .WriteTo.Console(new ElasticsearchJsonFormatter(inlineFields: true));
            })
            .ConfigureServices(async services => {
                var serviceProvider = services.BuildServiceProvider();
                var test2 = serviceProvider.GetService<IOptions<CronJobConfigurationOptions>>().Value.SyncBalanceCronSchedule;
                await SchedulerConfiguration.Configure(services);
            });
    }
}
