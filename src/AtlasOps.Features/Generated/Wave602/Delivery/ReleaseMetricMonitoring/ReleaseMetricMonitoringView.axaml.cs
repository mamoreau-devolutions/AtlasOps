namespace AtlasOps.Features.Delivery.ReleaseMetricMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseMetricMonitoringView : UserControl
{
    public ReleaseMetricMonitoringView()
    {
        this.DataContext = new ReleaseMetricMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseMetricMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}