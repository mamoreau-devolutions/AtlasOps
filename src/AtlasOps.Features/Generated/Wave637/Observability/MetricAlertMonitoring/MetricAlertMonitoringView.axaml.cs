namespace AtlasOps.Features.Observability.MetricAlertMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricAlertMonitoringView : UserControl
{
    public MetricAlertMonitoringView()
    {
        this.DataContext = new MetricAlertMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricAlertMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}