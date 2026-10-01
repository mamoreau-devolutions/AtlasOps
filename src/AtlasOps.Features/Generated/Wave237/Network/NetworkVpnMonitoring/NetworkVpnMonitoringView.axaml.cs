namespace AtlasOps.Features.Network.NetworkVpnMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkVpnMonitoringView : UserControl
{
    public NetworkVpnMonitoringView()
    {
        this.DataContext = new NetworkVpnMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkVpnMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}