using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AggregatesModel.CronJobAggregate
{
    public enum JobStatus
    {
        Dormant,
        InProgress,
        Completed,
        Failed,
    }

    public class CronJob : AuditableEntity, IAggregateRoot
    {
        public Guid CronJobId { get; set; }
        public string JobName { get; set; }
        public JobStatus Status { get; set; }

        public CronJob(
            Guid cronJobId,
            string jobName,
            JobStatus status = JobStatus.Dormant,
            string createdBy = null,
            string createdByName = null,
            DateTime? createdUTCDateTime = null,
            string modifiedBy = null,
            string modifiedByName = null,
            DateTime? modifiedUTCDateTime = null)
            : base(
                  createdBy: createdBy,
                  createdByName: createdByName,
                  createdUTCDateTime: createdUTCDateTime,
                  modifiedBy: modifiedBy,
                  modifiedByName: modifiedByName,
                  modifiedUTCDateTime: modifiedUTCDateTime
            )
        {
            CronJobId = cronJobId;
            JobName = jobName;
        }

        public void SetInProgress()
        {
            Status = JobStatus.InProgress;
        }

        public void SetCompleted()
        {
            Status = JobStatus.Completed;
        }

        public void SetFailed()
        {
            Status = JobStatus.Failed;
        }
    }
}
