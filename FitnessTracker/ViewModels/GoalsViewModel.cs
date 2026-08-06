using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessTracker.Models;
using FitnessTracker.Services;

namespace FitnessTracker.ViewModels
{
    public partial class GoalsViewModel : ObservableObject
    {
        public ObservableCollection<string> GoalTypes { get; } = new()
        {
            "Weight",
            "Calories",
        };
        public ObservableCollection<string> TimeFrames { get; } = new()
        {
            "Day",
            "Week",
            "Month",
            "Year"
        };
        public ObservableCollection<Goal> Goals => FitnessService.Goals;

        [ObservableProperty]
        private string selectedGoalType = "Weight";
        [ObservableProperty]
        private string selectedTimeFrame = "Day";
        [ObservableProperty]
        private string goal = "";
        [RelayCommand]
        private void AddGoal()
        {
            if(!double.TryParse(Goal, out double value)){
                return;
            }
            FitnessService.AddGoal(new Goal
            {
                GoalType = SelectedGoalType,
                TargetGoal = value,
                TimeFrame = SelectedTimeFrame,
                CreatedDate = DateTime.Today,
            });
            Goal = "";
        }
        [RelayCommand]
        private void DeleteGoal(Goal goal)
        {
            FitnessService.DeleteGoal(goal);
        }

    }
}
