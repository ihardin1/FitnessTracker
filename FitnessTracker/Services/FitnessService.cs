using FitnessTracker.Models;
using System.Collections.ObjectModel;

namespace FitnessTracker.Services;

public static class FitnessService
{
    public static ObservableCollection<Exercise> Exercises { get; } = new();

    public static ObservableCollection<Meal> Meals { get; } = new();

    public static ObservableCollection<CalorieEntry> Calories { get; } = new();
}