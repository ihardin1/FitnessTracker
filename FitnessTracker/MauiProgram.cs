using FitnessTracker.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace FitnessTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .Services.AddSingleton<HomeViewModel>();

            // Refresh today's workout on lifecycle events (app resume / activate)
            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android.OnResume(activity => FitnessTracker.Services.FitnessService.RefreshTodayWorkout()));
#endif
#if IOS
                events.AddiOS(ios => ios.FinishedLaunching((app, options) => { FitnessTracker.Services.FitnessService.RefreshTodayWorkout(); return true; }));
                events.AddiOS(ios => ios.OnActivated(application => FitnessTracker.Services.FitnessService.RefreshTodayWorkout()));
#endif
#if WINDOWS
                events.AddWindows(windows => windows.OnActivated((window, args) => FitnessTracker.Services.FitnessService.RefreshTodayWorkout()));
#endif
            });


    		builder.Logging.AddDebug();

            return builder.Build();
        }
    }
}
