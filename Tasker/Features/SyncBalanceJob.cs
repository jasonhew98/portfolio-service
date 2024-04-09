using Domain.AggregatesModel.CronJobAggregate;
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
                var cronJob = new CronJob(
                    cronJobId: Guid.NewGuid(),
                    jobName: typeof(SyncBalanceJob).Name);



                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }
    }
}
