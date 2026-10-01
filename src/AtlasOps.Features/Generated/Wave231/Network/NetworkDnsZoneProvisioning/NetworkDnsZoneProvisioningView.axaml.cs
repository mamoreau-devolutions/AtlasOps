namespace AtlasOps.Features.Network.NetworkDnsZoneProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkDnsZoneProvisioningView : UserControl
{
    public NetworkDnsZoneProvisioningView()
    {
        this.DataContext = new NetworkDnsZoneProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkDnsZoneProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}