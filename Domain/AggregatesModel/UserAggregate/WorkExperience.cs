using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AggregatesModel.UserAggregate
{
    public class WorkExperience
    {
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public bool IsCurrentJob { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }

        // Work specific skillsets
        public List<SkillSet> SkillSets { get; set; }
    }
}
