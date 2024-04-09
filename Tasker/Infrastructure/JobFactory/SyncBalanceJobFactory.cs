using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using Tasker.Features;

namespace Tasker.Infrastructure.JobFactory
{
    public class SyncBalanceJobFactory : IJobFactory
    {
        readonly IServiceProvider _serviceProvider;

        public SyncBalanceJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                return _serviceProvider.GetService<SyncBalanceJob>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
