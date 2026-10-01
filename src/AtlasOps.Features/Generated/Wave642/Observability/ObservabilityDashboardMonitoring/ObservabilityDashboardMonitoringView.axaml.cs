namespace AtlasOps.Features.Observability.ObservabilityDashboardMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityDashboardMonitoringView : UserControl
{
    public ObservabilityDashboardMonitoringView()
    {
        this.DataContext = new ObservabilityDashboardMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityDashboardMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}