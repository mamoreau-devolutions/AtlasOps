namespace AtlasOps.App.Views;

using AtlasOps.App.ViewModels;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        this.Opened += this.OnOpened;
        this.Closed += this.OnClosed;
    }

    private async void OnOpened(object? sender, EventArgs eventArgs)
    {
        this.Opened -= this.OnOpened;
        if (this.DataContext is MainViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    private async void OnClosed(object? sender, EventArgs eventArgs)
    {
        this.Closed -= this.OnClosed;
        if (this.DataContext is MainViewModel viewModel)
        {
            await viewModel.PersistLayoutAsync();
        }
    }
}