using FitnessTracker.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.ApplicationModel;

namespace FitnessTracker.Services;

public static class FitnessService
{
    static FitnessService()
    {
        // initialize weekly progress for the current week
        RefreshWeeklyProgress();
    }
    // Central collections for app state. Pages and viewmodels should read/update these collections.
    public static ObservableCollection<Exercise> Exercises { get; } = new();

    public static ObservableCollection<Meal> Meals { get; } = new();

    public static ObservableCollection<CalorieEntry> Calories { get; } = new();

    // UI-related collections exposed centrally so pages can populate or consume them.
    public static ObservableCollection<DayProgressModel> WeeklyProgress { get; } = new();

    public static ObservableCollection<ExerciseItemModel> TodayExercises { get; } = new();

    // Historical workouts and today's workout
    public static ObservableCollection<Workout> Workouts { get; } = new();

    private static Workout? todayWorkout;
    public static Workout? TodayWorkout
    {
        get => todayWorkout;
        private set
        {
            todayWorkout = value;
            OnTodayWorkoutChanged?.Invoke(todayWorkout);
        }
    }

    // Event fired when TodayWorkout changes. ViewModels/pages can subscribe to update UI.
    public static event Action<Workout?>? OnTodayWorkoutChanged;
    public static event Action? OnWorkoutsChanged;

    public static Workout? GetWorkoutForDate(DateTime date)
    {
        return Workouts.FirstOrDefault(w => w.Date.Date == date.Date);
    }

    public static void StartWorkoutForToday(Workout workout)
    {
        if (workout == null) return;
        workout.Date = DateTime.Today;
        workout.IsCompleted = false;
        TodayWorkout = workout;
        // Add to history (keep last instance for today if exists)
        var existing = GetWorkoutForDate(DateTime.Today);
        if (existing != null)
        {
            Workouts.Remove(existing);
        }
        Workouts.Add(workout);

        // Refresh weekly progress and notify
        RefreshWeeklyProgress();
        OnWorkoutsChanged?.Invoke();

        // Populate TodayExercises UI models
        TodayExercises.Clear();
        foreach (var ex in workout.Exercises)
        {
            TodayExercises.Add(new ExerciseItemModel { Name = ex.Name, Details = $"{ex.Sets} sets x {ex.Reps} reps" });
        }
    }

    public static void AddExerciseToToday(Exercise ex)
    {
        if (ex == null) return;
        if (TodayWorkout == null)
        {
            // create a default workout if none exists
            var w = new Workout { WorkoutName = "Today", Date = DateTime.Today };
            StartWorkoutForToday(w);
        }

        ex.Date = DateTime.Today;
        TodayWorkout!.Exercises.Add(ex);
        Exercises.Add(ex);
        TodayExercises.Add(new ExerciseItemModel { Name = ex.Name, Details = $"{ex.Sets} sets x {ex.Reps} reps" });
        RefreshWeeklyProgress();
        OnWorkoutsChanged?.Invoke();
    }

    public static void UpdateExerciseInToday(Exercise updated)
    {
        if (updated == null || TodayWorkout == null) return;
        var existing = TodayWorkout.Exercises.FirstOrDefault(e => e.Name == updated.Name);
        if (existing != null)
        {
            existing.Weight = updated.Weight;
            existing.Reps = updated.Reps;
            existing.Sets = updated.Sets;
            existing.IsCompleted = updated.IsCompleted;
        }

        // Refresh TodayExercises UI model entry
        var ui = TodayExercises.FirstOrDefault(t => t.Name == updated.Name);
        if (ui != null)
        {
            ui.Details = $"{updated.Sets} sets x {updated.Reps} reps";
        }
    }

    public static void CompleteTodayWorkout()
    {
        if (TodayWorkout == null) return;
        foreach (var ex in TodayWorkout.Exercises)
            ex.IsCompleted = true;
        TodayWorkout.IsCompleted = true;
        RefreshWeeklyProgress();
        // Notify subscribers (TodayWorkout still available for review)
        OnTodayWorkoutChanged?.Invoke(TodayWorkout);
        OnWorkoutsChanged?.Invoke();
    }

    public static void ClearTodayWorkout()
    {
        if (TodayWorkout == null) return;
        // remove from history if present
        var existing = GetWorkoutForDate(DateTime.Today);
        if (existing != null) Workouts.Remove(existing);

        TodayWorkout = null;
        TodayExercises.Clear();
        RefreshWeeklyProgress();
        OnWorkoutsChanged?.Invoke();
    }

    private static void RefreshWeeklyProgress(DateTime? referenceDate = null)
    {
        var refDate = referenceDate ?? DateTime.Today;
        // determine Monday as start of week
        var diff = (7 + (refDate.DayOfWeek - DayOfWeek.Monday)) % 7;
        var monday = refDate.Date.AddDays(-diff);

        WeeklyProgress.Clear();
        for (int i = 0; i < 7; i++)
        {
            var date = monday.AddDays(i);
            var workout = GetWorkoutForDate(date);
            var label = date.ToString("ddd").Substring(0, 1);
            WeeklyProgress.Add(new DayProgressModel { DayLabel = label, IsCompleted = workout?.IsCompleted ?? false });
        }
    }

    // Public method to refresh today's workout and weekly progress. Call this on app resume/navigation.
    public static void RefreshTodayWorkout()
    {
        TodayWorkout = GetWorkoutForDate(DateTime.Today);
        RefreshWeeklyProgress();
        OnTodayWorkoutChanged?.Invoke(TodayWorkout);
        OnWorkoutsChanged?.Invoke();
    }
}
