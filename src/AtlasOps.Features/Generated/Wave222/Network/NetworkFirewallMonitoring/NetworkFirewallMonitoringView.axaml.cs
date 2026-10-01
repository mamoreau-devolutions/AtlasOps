namespace AtlasOps.Features.Network.NetworkFirewallMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkFirewallMonitoringView : UserControl
{
    public NetworkFirewallMonitoringView()
    {
        this.DataContext = new NetworkFirewallMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkFirewallMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}