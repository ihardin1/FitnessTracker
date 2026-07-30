using System.Collections.ObjectModel;

namespace FitnessTracker.Models;

public class Workout
{
    public string WorkoutName { get; set; } = string.Empty;

    public string Day { get; set; } = string.Empty;

    public ObservableCollection<Exercise> Exercises { get; set; } = new();
}