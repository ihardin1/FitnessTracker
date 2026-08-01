using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessTracker.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string todayWorkoutSummary = "Estimated: 55 mins • 3 exercises";

        public ObservableCollection<DayProgressModel> WeeklyProgress { get; set; } = new();
        public ObservableCollection<ExerciseItemModel> TodayExercises { get; set; } = new();

        public HomeViewModel()
        {
            LoadMockData();
        }

        private void LoadMockData()
        {
            // Populate weekly days (M-S)
            WeeklyProgress.Add(new DayProgressModel { DayLabel = "M", IsCompleted = false });
            WeeklyProgress.Add(new DayProgressModel { DayLabel = "T", IsCompleted = false });
            WeeklyProgress.Add(new DayProgressModel { DayLabel = "W", IsCompleted = true });
            WeeklyProgress.Add(new DayProgressModel { DayLabel = "T", IsCompleted = false });
            WeeklyProgress.Add(new DayProgressModel { DayLabel = "F", IsCompleted = false });
            WeeklyProgress.Add(new DayProgressModel { DayLabel = "S", IsCompleted = true });
            WeeklyProgress.Add(new DayProgressModel { DayLabel = "S", IsCompleted = true });

            // Populate today's exercise routine
            TodayExercises.Add(new ExerciseItemModel { Name = "Barbell Bench Press", Details = "4 sets x 8-10 reps" });
            TodayExercises.Add(new ExerciseItemModel { Name = "Incline Dumbbell Press", Details = "3 sets x 10 reps" });
            TodayExercises.Add(new ExerciseItemModel { Name = "Overhead Press", Details = "3 sets x 8 reps" });
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

    public class DayProgressModel
    {
        public string DayLabel { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    public class ExerciseItemModel
    {
        public string Name { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}