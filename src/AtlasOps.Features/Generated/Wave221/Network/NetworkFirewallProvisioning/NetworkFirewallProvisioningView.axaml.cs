namespace AtlasOps.Features.Network.NetworkFirewallProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkFirewallProvisioningView : UserControl
{
    public NetworkFirewallProvisioningView()
    {
        this.DataContext = new NetworkFirewallProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkFirewallProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}