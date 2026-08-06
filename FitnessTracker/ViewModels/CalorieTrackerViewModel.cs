using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessTracker.Models;
using FitnessTracker.Services;

namespace FitnessTracker.ViewModels;

public partial class CalorieTrackerViewModel : ObservableObject
{
    [ObservableProperty]
    private string foodName = string.Empty;

    [ObservableProperty]
    private string selectedMealType = "Breakfast";

    [ObservableProperty]
    private int calories;

    [ObservableProperty]
    private Meal? selectedMeal;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private int dailyCalorieGoal = 1500;

    [ObservableProperty]
    private int caloriesConsumed;

    [ObservableProperty]
    private int caloriesRemaining;

    [ObservableProperty]
    private double calorieProgress;

    public ObservableCollection<string> MealTypes { get; } =
        new()
        {
            "Breakfast",
            "Lunch",
            "Dinner",
            "Snack"
        };

    public ObservableCollection<CalorieEntry> TodayEntries { get; } =
        new();

    public ObservableCollection<Meal> Meals =>
        FitnessService.Meals;

    public CalorieTrackerViewModel()
    {
        Refresh();

        FitnessService.OnCaloriesChanged += Refresh;
    }

    partial void OnSelectedMealChanged(Meal? value)
    {
        if (value == null)
        {
            return;
        }

        FoodName = value.Name;
        Calories = value.Calories;
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        Refresh();
    }

    partial void OnDailyCalorieGoalChanged(int value)
    {
        CalculateTotals();
    }

    private void Refresh()
    {
        TodayEntries.Clear();

        IEnumerable<CalorieEntry> entries =
            FitnessService.Calories
                .Where(entry =>
                    entry.Date.Date == SelectedDate.Date)
                .OrderByDescending(entry => entry.Date);

        foreach (CalorieEntry entry in entries)
        {
            TodayEntries.Add(entry);
        }

        CalculateTotals();
    }

    private void CalculateTotals()
    {
        CaloriesConsumed =
            TodayEntries.Sum(entry => entry.Calories);

        CaloriesRemaining =
            Math.Max(
                0,
                DailyCalorieGoal - CaloriesConsumed);

        CalorieProgress =
            DailyCalorieGoal <= 0
                ? 0
                : Math.Min(
                    1,
                    (double)CaloriesConsumed /
                    DailyCalorieGoal);
    }

    [RelayCommand]
    private async Task AddCalories()
    {
        if (string.IsNullOrWhiteSpace(FoodName))
        {
            await Shell.Current.DisplayAlertAsync(
                "Missing Food",
                "Please enter a food or meal name.",
                "OK");

            return;
        }

        if (Calories <= 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Invalid Calories",
                "Calories must be greater than zero.",
                "OK");

            return;
        }

        CalorieEntry entry = new()
        {
            FoodName = FoodName.Trim(),
            MealType = SelectedMealType,
            Calories = Calories,
            Date = SelectedDate
        };

        FitnessService.AddCalorieEntry(entry);

        FoodName = string.Empty;
        Calories = 0;
        SelectedMeal = null;

        Refresh();
    }

    [RelayCommand]
    private async Task DeleteEntry(
        CalorieEntry? entry)
    {
        if (entry == null)
        {
            return;
        }

        bool shouldDelete =
            await Shell.Current.DisplayAlertAsync(
                "Delete Entry",
                $"Remove {entry.FoodName}?",
                "Delete",
                "Cancel");

        if (!shouldDelete)
        {
            return;
        }

        FitnessService.DeleteCalorieEntry(entry);

        Refresh();
    }
}