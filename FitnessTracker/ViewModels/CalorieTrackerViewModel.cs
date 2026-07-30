namespace FitnessTracker.ViewModels;

public class CalorieTrackerViewModel : ContentPage
{
	public CalorieTrackerViewModel()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
				}
			}
		};
	}
}