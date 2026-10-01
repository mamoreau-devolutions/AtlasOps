namespace AtlasOps.Features.Network.NetworkRouteMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkRouteMonitoringView : UserControl
{
    public NetworkRouteMonitoringView()
    {
        this.DataContext = new NetworkRouteMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkRouteMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}