namespace AtlasOps.Features.Network.NetworkPeerMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPeerMonitoringView : UserControl
{
    public NetworkPeerMonitoringView()
    {
        this.DataContext = new NetworkPeerMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPeerMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}