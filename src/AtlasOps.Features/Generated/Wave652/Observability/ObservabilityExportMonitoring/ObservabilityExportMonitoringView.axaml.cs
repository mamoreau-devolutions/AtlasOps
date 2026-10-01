namespace AtlasOps.Features.Observability.ObservabilityExportMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityExportMonitoringView : UserControl
{
    public ObservabilityExportMonitoringView()
    {
        this.DataContext = new ObservabilityExportMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityExportMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}