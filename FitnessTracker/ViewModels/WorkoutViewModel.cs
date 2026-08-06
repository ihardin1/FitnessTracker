using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessTracker.Models;
using FitnessTracker.Services;

namespace FitnessTracker.ViewModels;

public partial class WorkoutViewModel : ObservableObject
{
    [ObservableProperty]
    private string exerciseName = string.Empty;

    [ObservableProperty]
    private double weight;

    [ObservableProperty]
    private int sets = 3;

    [ObservableProperty]
    private int reps = 10;

    [ObservableProperty]
    private string workoutTitle = "Today's Workout";

    public ObservableCollection<Exercise> Exercises { get; } = new();

    public int TotalExercises => Exercises.Count;

    public int TotalSets => Exercises.Sum(exercise => exercise.Sets);

    public double TotalVolume =>
        Exercises.Sum(
            exercise =>
                exercise.Weight *
                exercise.Reps *
                exercise.Sets);

    public WorkoutViewModel()
    {
        Refresh();

        FitnessService.OnWorkoutsChanged += Refresh;
        FitnessService.OnTodayWorkoutChanged += OnTodayWorkoutChanged;
    }

    private void OnTodayWorkoutChanged(Workout? workout)
    {
        Refresh();
    }

    private void Refresh()
    {
        Exercises.Clear();

        Workout? todayWorkout = FitnessService.TodayWorkout;

        if (todayWorkout?.Exercises != null)
        {
            foreach (Exercise exercise in todayWorkout.Exercises)
            {
                Exercises.Add(exercise);
            }
        }

        OnPropertyChanged(nameof(TotalExercises));
        OnPropertyChanged(nameof(TotalSets));
        OnPropertyChanged(nameof(TotalVolume));
    }

    [RelayCommand]
    private async Task AddExercise()
    {
        if (string.IsNullOrWhiteSpace(ExerciseName))
        {
            await Shell.Current.DisplayAlertAsync(
                "Missing Exercise",
                "Please enter an exercise name.",
                "OK");

            return;
        }

        if (Sets <= 0 || Reps <= 0 || Weight < 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Invalid Information",
                "Sets and reps must be greater than zero. Weight cannot be negative.",
                "OK");

            return;
        }

        Exercise exercise = new()
        {
            Name = ExerciseName.Trim(),
            Weight = Weight,
            Sets = Sets,
            Reps = Reps,
            Date = DateTime.Today,
            IsCompleted = false
        };

        FitnessService.AddExerciseToToday(exercise);

        ExerciseName = string.Empty;
        Weight = 0;
        Sets = 3;
        Reps = 10;

        Refresh();
    }

    [RelayCommand]
    private async Task DeleteExercise(Exercise? exercise)
    {
        if (exercise == null)
        {
            return;
        }

        bool shouldDelete =
            await Shell.Current.DisplayAlertAsync(
                "Delete Exercise",
                $"Remove {exercise.Name} from today's workout?",
                "Delete",
                "Cancel");

        if (!shouldDelete)
        {
            return;
        }

        FitnessService.DeleteExerciseFromToday(exercise);

        Refresh();
    }

    [RelayCommand]
    private async Task FinishWorkout()
    {
        if (Exercises.Count == 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "No Exercises",
                "Add at least one exercise before completing your workout.",
                "OK");

            return;
        }

        FitnessService.CompleteTodayWorkout();

        Refresh();

        await Shell.Current.DisplayAlertAsync(
            "Workout Complete 🎉",
            $"You completed {TotalExercises} exercises and {TotalSets} total sets.",
            "Great!");
    }
}