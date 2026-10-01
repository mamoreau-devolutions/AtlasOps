namespace AtlasOps.Features.Edge.EdgeIncidentMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeIncidentMonitoringView : UserControl
{
    public EdgeIncidentMonitoringView()
    {
        this.DataContext = new EdgeIncidentMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeIncidentMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}