using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AggregatesModel.UserAggregate
{
    public class SkillSet
    {
        public string Name { get; set; }
        public SkillType SkillType { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; }
    }

    public enum SkillType
    {
        Frontend,
        Backend
    }

    public enum ProficiencyLevel
    {
        Beginner,
        Intermediate,
        Expert
    }
}
