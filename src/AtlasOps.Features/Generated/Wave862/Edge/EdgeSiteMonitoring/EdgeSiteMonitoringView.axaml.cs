namespace AtlasOps.Features.Edge.EdgeSiteMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeSiteMonitoringView : UserControl
{
    public EdgeSiteMonitoringView()
    {
        this.DataContext = new EdgeSiteMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeSiteMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}