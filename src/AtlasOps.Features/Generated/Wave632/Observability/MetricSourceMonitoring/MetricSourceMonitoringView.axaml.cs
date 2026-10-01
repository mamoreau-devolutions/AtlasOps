namespace AtlasOps.Features.Observability.MetricSourceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricSourceMonitoringView : UserControl
{
    public MetricSourceMonitoringView()
    {
        this.DataContext = new MetricSourceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricSourceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}