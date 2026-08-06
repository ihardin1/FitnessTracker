using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Models
{
    public class Goal
    {
        public string GoalType { get; set; } = "Weight";

        public double TargetGoal { get; set; }

        public string TimeFrame { get; set; } = "Month";

        public DateTime CreatedDate { get; set; } = DateTime.Today;

        public string GoalDisplay =>
            GoalType == "Weight" ? $"{TargetGoal:0.#} lb"
            : $"{TargetGoal:0.#} cal";
    }
}
