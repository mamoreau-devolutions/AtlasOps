namespace AtlasOps.Features.Edge.EdgeTelemetryMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeTelemetryMonitoringView : UserControl
{
    public EdgeTelemetryMonitoringView()
    {
        this.DataContext = new EdgeTelemetryMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeTelemetryMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}