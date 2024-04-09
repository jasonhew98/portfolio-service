using Microsoft.Extensions.Options;
using Quartz;
using Tasker.Infrastructure;

namespace Tasker.Features
{
    public class SyncBalanceJob : IJob
    {
        private readonly CronJobConfigurationOptions _configuration;

        public SyncBalanceJob(
            IOptions<CronJobConfigurationOptions> configuration)
        {
            _configuration = configuration.Value;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine("Error configuring scheduler.");
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }
    }
}
