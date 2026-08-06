using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage()
    {
        InitializeComponent();

        _viewModel = new HomeViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        BindingContext = _viewModel;
    }
}