using System.Collections.ObjectModel;

namespace FitnessTracker.Models;

public class Workout
{
    public string WorkoutName { get; set; } = string.Empty;

    
    public DateTime Date { get; set; } = DateTime.Today;

    public ObservableCollection<Exercise> Exercises { get; set; } = new();

    public bool IsCompleted { get; set; }
}