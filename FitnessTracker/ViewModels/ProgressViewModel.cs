using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FitnessTracker.Models;
using FitnessTracker.Services;

namespace FitnessTracker.ViewModels;

public partial class ProgressViewModel : ObservableObject
{
    [ObservableProperty]
    private int totalWorkouts;

    [ObservableProperty]
    private int totalExercises;

    [ObservableProperty]
    private double totalVolume;

    [ObservableProperty]
    private string favoriteExercise = "No workouts yet";

    public ObservableCollection<Exercise> ExerciseHistory { get; } = new();

    public ProgressViewModel()
    {
        Refresh();

        FitnessService.OnWorkoutsChanged += Refresh;
    }

    private void Refresh()
    {
        ExerciseHistory.Clear();

        foreach (Exercise exercise in FitnessService.Exercises
                     .OrderByDescending(x => x.Date))
        {
            ExerciseHistory.Add(exercise);
        }

        TotalWorkouts = FitnessService.Workouts.Count;

        TotalExercises = FitnessService.Exercises.Count;

        TotalVolume = FitnessService.Exercises.Sum(
            exercise =>
                exercise.Weight *
                exercise.Reps *
                exercise.Sets);

        FavoriteExercise = FitnessService.Exercises
            .GroupBy(exercise => exercise.Name)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .FirstOrDefault() ?? "No workouts yet";
    }
}