using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AggregatesModel.UserAggregate
{
    public class SkillSet
    {
        public string Name { get; set; }
        public int ProficiencyLevel { get; set; }
    }
}
