using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessTracker.Models;
using FitnessTracker.Services;

namespace FitnessTracker.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private string greeting = "";

    [ObservableProperty]
    private string currentDate = "";

    [ObservableProperty]
    private string todayWorkoutSummary = "";

    [ObservableProperty]
    private int totalCaloriesToday;

    [ObservableProperty]
    private int totalExercises;

    [ObservableProperty]
    private int totalWorkouts;

    public ObservableCollection<DayProgressModel> WeeklyProgress =>
        FitnessService.WeeklyProgress;

    public ObservableCollection<ExerciseItemModel> TodayExercises =>
        FitnessService.TodayExercises;

    public HomeViewModel()
    {
        Refresh();

        FitnessService.OnTodayWorkoutChanged += _ => Refresh();
        FitnessService.OnWorkoutsChanged += Refresh;
        FitnessService.OnCaloriesChanged += Refresh;
    }

    private void Refresh()
    {
        CurrentDate = DateTime.Now.ToString("dddd, MMMM d");

        Greeting = DateTime.Now.Hour switch
        {
            < 12 => "🌅 Good Morning",
            < 17 => "☀️ Good Afternoon",
            _ => "🌙 Good Evening"
        };

        TotalCaloriesToday = FitnessService.GetCaloriesForToday();

        TotalExercises = FitnessService.Exercises.Count;

        TotalWorkouts = FitnessService.Workouts.Count;

        if (FitnessService.TodayWorkout == null)
        {
            TodayWorkoutSummary = "No workout scheduled";
        }
        else
        {
            int exerciseCount = FitnessService.TodayWorkout.Exercises.Count;

            int totalSets = FitnessService.TodayWorkout.Exercises.Sum(x => x.Sets);

            TodayWorkoutSummary =
                $"{exerciseCount} exercises • {totalSets} sets";
        }

        OnPropertyChanged(nameof(WeeklyProgress));
        OnPropertyChanged(nameof(TodayExercises));
    }

    [RelayCommand]
    private async Task QuickStart()
    {
        await Shell.Current.GoToAsync("//WorkoutPage");
    }

    [RelayCommand]
    private async Task LaunchWorkout()
    {
        await Shell.Current.GoToAsync("//WorkoutPage");
    }

    [RelayCommand]
    private async Task ChangeRoutine()
    {
        await Shell.Current.DisplayAlert(
            "Workout",
            "Routine switching will be available in a future version.",
            "OK");
    }
}