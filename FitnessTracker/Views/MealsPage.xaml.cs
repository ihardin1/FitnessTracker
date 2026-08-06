using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class MealsPage : ContentPage
{
    private readonly MealsViewModel viewModel;

    public MealsPage()
    {
        InitializeComponent();

        viewModel = new MealsViewModel();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        BindingContext = viewModel;
    }
}