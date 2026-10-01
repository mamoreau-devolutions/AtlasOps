namespace AtlasOps.Features.Edge.EdgeNetworkMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeNetworkMonitoringView : UserControl
{
    public EdgeNetworkMonitoringView()
    {
        this.DataContext = new EdgeNetworkMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeNetworkMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}