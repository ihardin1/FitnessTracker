using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessTracker.Services;
using FitnessTracker.Models;

namespace FitnessTracker.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string todayWorkoutSummary = "Estimated: 55 mins • 3 exercises";

        public ObservableCollection<DayProgressModel> WeeklyProgress { get; set; }
        public ObservableCollection<ExerciseItemModel> TodayExercises { get; set; }

        public HomeViewModel()
        {
            // Use central service collections so data can be provided by other pages/views
            WeeklyProgress = FitnessService.WeeklyProgress;
            TodayExercises = FitnessService.TodayExercises;
        // initialize summary from service
        UpdateTodaySummary(FitnessService.TodayWorkout);
        FitnessService.OnTodayWorkoutChanged += UpdateTodaySummary;
        }

    private void UpdateTodaySummary(Workout? workout)
    {
        if (workout == null)
        {
            TodayWorkoutSummary = "No workout for today";
            return;
        }

        var exerciseCount = workout.Exercises?.Count ?? 0;
        var totalSets = workout.Exercises?.Sum(e => e.Sets) ?? 0;
        TodayWorkoutSummary = $"{exerciseCount} exercises • {totalSets} sets";
    }

        [RelayCommand]
        private void QuickStart()
        {
            // Handle quick start logic
        }

        [RelayCommand]
        private void LaunchWorkout()
        {
            // Handle launch routine logic
        }

        [RelayCommand]
        private void ChangeRoutine()
        {
            // Handle routine swapping logic
        }
    }

    // UI models moved to FitnessTracker.Models.UiModels
}