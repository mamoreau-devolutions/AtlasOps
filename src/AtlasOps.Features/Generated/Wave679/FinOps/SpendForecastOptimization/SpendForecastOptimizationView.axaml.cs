namespace AtlasOps.Features.FinOps.SpendForecastOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SpendForecastOptimizationView : UserControl
{
    public SpendForecastOptimizationView()
    {
        this.DataContext = new SpendForecastOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SpendForecastOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}