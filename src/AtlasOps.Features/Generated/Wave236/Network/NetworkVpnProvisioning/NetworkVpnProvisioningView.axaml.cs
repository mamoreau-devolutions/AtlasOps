namespace AtlasOps.Features.Network.NetworkVpnProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkVpnProvisioningView : UserControl
{
    public NetworkVpnProvisioningView()
    {
        this.DataContext = new NetworkVpnProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkVpnProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}