using System.Collections.ObjectModel;
using System.Text.Json;
using FitnessTracker.Models;

namespace FitnessTracker.Services;

public static class FitnessService
{
   
    private const string WorkoutsFileName = "workouts.json";
    private const string MealsFileName = "meals.json";
    private const string CaloriesFileName = "calories.json";
    private const string GoalsFileName = "goals.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static ObservableCollection<Exercise> Exercises { get; } = new();

    public static ObservableCollection<Meal> Meals { get; } = new();

    public static ObservableCollection<CalorieEntry> Calories { get; } = new();

    public static ObservableCollection<Workout> Workouts { get; } = new();

    public static ObservableCollection<Goal> Goals { get; } = new();

    public static ObservableCollection<DayProgressModel> WeeklyProgress { get; } = new();

    public static ObservableCollection<ExerciseItemModel> TodayExercises { get; } = new();

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

    public static event Action<Workout?>? OnTodayWorkoutChanged;

    public static event Action? OnWorkoutsChanged;

    public static event Action? OnCaloriesChanged;

    public static event Action? OnMealsChanged;

    public static event Action? OnGoalsChanged;

    static FitnessService()
    {
        LoadAllData();

        RefreshTodayWorkout();
    }

    public static Workout? GetWorkoutForDate(DateTime date)
    {
        return Workouts.FirstOrDefault(
            workout => workout.Date.Date == date.Date);
    }

    public static void StartWorkoutForToday(Workout workout)
    {
        if (workout == null)
        {
            return;
        }

        workout.Date = DateTime.Today;
        workout.IsCompleted = false;

        Workout? existingWorkout =
            GetWorkoutForDate(DateTime.Today);

        if (existingWorkout != null)
        {
            Workouts.Remove(existingWorkout);
        }

        Workouts.Add(workout);

        TodayWorkout = workout;

        RefreshTodayExercises();
        RefreshWeeklyProgress();

        SaveWorkouts();

        OnWorkoutsChanged?.Invoke();
    }

    public static void AddExerciseToToday(Exercise exercise)
    {
        if (exercise == null)
        {
            return;
        }

        if (TodayWorkout == null)
        {
            Workout newWorkout = new()
            {
                WorkoutName = "Today's Workout",
                Date = DateTime.Today,
                IsCompleted = false
            };

            StartWorkoutForToday(newWorkout);
        }

        exercise.Date = DateTime.Today;

        TodayWorkout!.Exercises.Add(exercise);

        Exercises.Add(exercise);

        RefreshTodayExercises();
        RefreshWeeklyProgress();

        SaveWorkouts();

        OnTodayWorkoutChanged?.Invoke(TodayWorkout);
        OnWorkoutsChanged?.Invoke();
    }

    public static void UpdateExerciseInToday(
        Exercise originalExercise,
        Exercise updatedExercise)
    {
        if (originalExercise == null ||
            updatedExercise == null ||
            TodayWorkout == null)
        {
            return;
        }

        Exercise? existingExercise =
            TodayWorkout.Exercises.FirstOrDefault(
                exercise => exercise == originalExercise);

        if (existingExercise == null)
        {
            existingExercise =
                TodayWorkout.Exercises.FirstOrDefault(
                    exercise =>
                        exercise.Name == originalExercise.Name);
        }

        if (existingExercise == null)
        {
            return;
        }

        existingExercise.Name = updatedExercise.Name;
        existingExercise.Weight = updatedExercise.Weight;
        existingExercise.Reps = updatedExercise.Reps;
        existingExercise.Sets = updatedExercise.Sets;
        existingExercise.Date = updatedExercise.Date;
        existingExercise.IsCompleted =
            updatedExercise.IsCompleted;

        RefreshTodayExercises();
        RefreshWeeklyProgress();

        SaveWorkouts();

        OnTodayWorkoutChanged?.Invoke(TodayWorkout);
        OnWorkoutsChanged?.Invoke();
    }

   
    public static void UpdateExerciseInToday(
        Exercise updatedExercise)
    {
        if (updatedExercise == null ||
            TodayWorkout == null)
        {
            return;
        }

        Exercise? existingExercise =
            TodayWorkout.Exercises.FirstOrDefault(
                exercise =>
                    exercise.Name == updatedExercise.Name);

        if (existingExercise == null)
        {
            return;
        }

        existingExercise.Weight = updatedExercise.Weight;
        existingExercise.Reps = updatedExercise.Reps;
        existingExercise.Sets = updatedExercise.Sets;
        existingExercise.IsCompleted =
            updatedExercise.IsCompleted;

        RefreshTodayExercises();
        RefreshWeeklyProgress();

        SaveWorkouts();

        OnTodayWorkoutChanged?.Invoke(TodayWorkout);
        OnWorkoutsChanged?.Invoke();
    }

    public static void DeleteExerciseFromToday(
        Exercise exercise)
    {
        if (exercise == null ||
            TodayWorkout == null)
        {
            return;
        }

        TodayWorkout.Exercises.Remove(exercise);

        Exercise? savedExercise =
            Exercises.FirstOrDefault(
                item =>
                    item.Name == exercise.Name &&
                    item.Date.Date == exercise.Date.Date);

        if (savedExercise != null)
        {
            Exercises.Remove(savedExercise);
        }

        RefreshTodayExercises();
        RefreshWeeklyProgress();

        SaveWorkouts();

        OnTodayWorkoutChanged?.Invoke(TodayWorkout);
        OnWorkoutsChanged?.Invoke();
    }

    public static void CompleteTodayWorkout()
    {
        if (TodayWorkout == null)
        {
            return;
        }

        foreach (Exercise exercise in TodayWorkout.Exercises)
        {
            exercise.IsCompleted = true;
        }

        TodayWorkout.IsCompleted = true;

        RefreshTodayExercises();
        RefreshWeeklyProgress();

        SaveWorkouts();

        OnTodayWorkoutChanged?.Invoke(TodayWorkout);
        OnWorkoutsChanged?.Invoke();
    }

    public static void DeleteWorkout(Workout workout)
    {
        if (workout == null)
        {
            return;
        }

        Workouts.Remove(workout);

        if (TodayWorkout == workout ||
            workout.Date.Date == DateTime.Today)
        {
            TodayWorkout = null;
        }

        RebuildExerciseHistory();
        RefreshTodayExercises();
        RefreshWeeklyProgress();

        SaveWorkouts();

        OnWorkoutsChanged?.Invoke();
    }

    public static void ClearTodayWorkout()
    {
        Workout? existingWorkout =
            GetWorkoutForDate(DateTime.Today);

        if (existingWorkout != null)
        {
            Workouts.Remove(existingWorkout);
        }

        TodayWorkout = null;

        RebuildExerciseHistory();

        TodayExercises.Clear();

        RefreshWeeklyProgress();

        SaveWorkouts();

        OnWorkoutsChanged?.Invoke();
    }

  

    public static void AddCalorieEntry(
        CalorieEntry calorieEntry)
    {
        if (calorieEntry == null)
        {
            return;
        }

        calorieEntry.Date = calorieEntry.Date.Date;

        Calories.Add(calorieEntry);

        SaveCalories();

        OnCaloriesChanged?.Invoke();
    }

    public static void DeleteCalorieEntry(
        CalorieEntry calorieEntry)
    {
        if (calorieEntry == null)
        {
            return;
        }

        Calories.Remove(calorieEntry);

        SaveCalories();

        OnCaloriesChanged?.Invoke();
    }

    public static int GetCaloriesForDate(
        DateTime date)
    {
        return Calories
            .Where(entry =>
                entry.Date.Date == date.Date)
            .Sum(entry => entry.Calories);
    }

    public static int GetCaloriesForToday()
    {
        return GetCaloriesForDate(DateTime.Today);
    }

   
    public static void AddMeal(Meal meal)
    {
        if (meal == null)
        {
            return;
        }

        Meals.Add(meal);

        SaveMeals();

        OnMealsChanged?.Invoke();
    }

    public static void DeleteMeal(Meal meal)
    {
        if (meal == null)
        {
            return;
        }

        Meals.Remove(meal);

        SaveMeals();

        OnMealsChanged?.Invoke();
    }

    public static void RefreshTodayWorkout()
    {
        TodayWorkout =
            GetWorkoutForDate(DateTime.Today);

        RebuildExerciseHistory();
        RefreshTodayExercises();
        RefreshWeeklyProgress();

        OnWorkoutsChanged?.Invoke();
    }

    public static void AddGoal(Goal goal)
    {
        if (goal == null) return;
        Goals.Add(goal);
        SaveGoals();
        OnGoalsChanged?.Invoke();
    }

    public static void DeleteGoal(Goal goal)
    {
        if (goal == null) return;
        Goals.Remove(goal);
        SaveGoals();
        OnGoalsChanged?.Invoke();
    }

    private static void RefreshTodayExercises()
    {
        TodayExercises.Clear();

        if (TodayWorkout == null)
        {
            return;
        }

        foreach (Exercise exercise
                 in TodayWorkout.Exercises)
        {
            TodayExercises.Add(
                new ExerciseItemModel
                {
                    Name = exercise.Name,

                    Details =
                        $"{exercise.Sets} sets × " +
                        $"{exercise.Reps} reps • " +
                        $"{exercise.Weight:0.#} lb"
                });
        }
    }

    private static void RefreshWeeklyProgress(
        DateTime? referenceDate = null)
    {
        DateTime selectedDate =
            referenceDate ?? DateTime.Today;

        int daysFromMonday =
            (7 +
             (selectedDate.DayOfWeek -
              DayOfWeek.Monday)) % 7;

        DateTime monday =
            selectedDate.Date.AddDays(
                -daysFromMonday);

        WeeklyProgress.Clear();

        for (int dayNumber = 0;
             dayNumber < 7;
             dayNumber++)
        {
            DateTime date =
                monday.AddDays(dayNumber);

            Workout? workout =
                GetWorkoutForDate(date);

            WeeklyProgress.Add(
                new DayProgressModel
                {
                    DayLabel =
                        date.ToString("ddd")[..1],

                    IsCompleted =
                        workout?.IsCompleted ?? false
                });
        }
    }

    private static void RebuildExerciseHistory()
    {
        Exercises.Clear();

        foreach (Workout workout in Workouts)
        {
            foreach (Exercise exercise
                     in workout.Exercises)
            {
                Exercises.Add(exercise);
            }
        }
    }


    public static void SaveAllData()
    {
        SaveWorkouts();
        SaveMeals();
        SaveCalories();
        SaveGoals();
    }

    private static void LoadAllData()
    {
        LoadWorkouts();
        LoadMeals();
        LoadCalories();
        LoadGoals();
        RebuildExerciseHistory();
    }

    private static void SaveWorkouts()
    {
        SaveCollection(
            WorkoutsFileName,
            Workouts.ToList());
    }

    private static void SaveMeals()
    {
        SaveCollection(
            MealsFileName,
            Meals.ToList());
    }

    private static void SaveCalories()
    {
        SaveCollection(
            CaloriesFileName,
            Calories.ToList());
    }

    private static void SaveGoals()
    {
        SaveCollection(
            GoalsFileName,
            Goals.ToList());
    }

    private static void LoadWorkouts()
    {
        List<Workout> savedWorkouts =
            LoadCollection<Workout>(
                WorkoutsFileName);

        Workouts.Clear();

        foreach (Workout workout
                 in savedWorkouts)
        {
            Workouts.Add(workout);
        }
    }

    private static void LoadMeals()
    {
        List<Meal> savedMeals =
            LoadCollection<Meal>(
                MealsFileName);

        Meals.Clear();

        foreach (Meal meal in savedMeals)
        {
            Meals.Add(meal);
        }
    }

    private static void LoadCalories()
    {
        List<CalorieEntry> savedCalories =
            LoadCollection<CalorieEntry>(
                CaloriesFileName);

        Calories.Clear();

        foreach (CalorieEntry calorie
                 in savedCalories)
        {
            Calories.Add(calorie);
        }
    }

    private static void LoadGoals()
    {
        List<Goal> savedGoals =
            LoadCollection<Goal>(
                GoalsFileName);
        Goals.Clear();
        foreach (Goal goal in savedGoals)
        {
            Goals.Add(goal);
        }
    }

    private static void SaveCollection<T>(
        string fileName,
        List<T> items)
    {
        try
        {
            string filePath = Path.Combine(
                FileSystem.AppDataDirectory,
                fileName);

            string json =
                JsonSerializer.Serialize(
                    items,
                    JsonOptions);

            File.WriteAllText(
                filePath,
                json);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error saving {fileName}: " +
                exception.Message);
        }
    }

    private static List<T> LoadCollection<T>(
        string fileName)
    {
        try
        {
            string filePath = Path.Combine(
                FileSystem.AppDataDirectory,
                fileName);

            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            string json =
                File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<T>>(
                       json,
                       JsonOptions)
                   ?? new List<T>();
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error loading {fileName}: " +
                exception.Message);

            return new List<T>();
        }
    }
}