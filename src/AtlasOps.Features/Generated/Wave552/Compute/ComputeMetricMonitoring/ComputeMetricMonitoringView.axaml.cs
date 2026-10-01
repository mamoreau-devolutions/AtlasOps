namespace AtlasOps.Features.Compute.ComputeMetricMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeMetricMonitoringView : UserControl
{
    public ComputeMetricMonitoringView()
    {
        this.DataContext = new ComputeMetricMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeMetricMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}