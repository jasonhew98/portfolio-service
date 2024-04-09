using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Tasker.Features;
using Tasker.Infrastructure.JobFactory;

namespace Tasker.Infrastructure
{
    public static class SchedulerConfiguration
    {

        public static async Task<IScheduler> ConfigureSyncBalance(ServiceProvider serviceProvider, IScheduler scheduler)
        {
            try
            {
                var cronSchedule = serviceProvider.GetService<IOptions<CronJobConfigurationOptions>>().Value.SyncBalanceCronSchedule;

                var job = JobBuilder.Create<SyncBalanceJob>()
                    .WithIdentity(typeof(SyncBalanceJob).Name)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{typeof(SyncBalanceJob).Name}.Trigger")
                    .WithCronSchedule(cronSchedule)
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                return scheduler;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public static async Task Configure(IServiceCollection services)
        {
            try
            {
                var serviceProvider = services.BuildServiceProvider();

                var factory = new StdSchedulerFactory();
                var scheduler = await factory.GetScheduler();
                scheduler.JobFactory = new SyncBalanceJobFactory(serviceProvider);
                await ConfigureSyncBalance(serviceProvider, scheduler);

                await scheduler.Start();

                await Task.Delay(TimeSpan.FromSeconds(1));
                
            }
            catch(Exception e)
            {
                Console.WriteLine("Error configuring scheduler. " + e.Message);
            }
        }
    }
}
