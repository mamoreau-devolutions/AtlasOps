namespace AtlasOps.Features.FinOps.SpendForecastMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SpendForecastMonitoringView : UserControl
{
    public SpendForecastMonitoringView()
    {
        this.DataContext = new SpendForecastMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SpendForecastMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}