namespace AtlasOps.Features.Network.NetworkPeerProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPeerProvisioningView : UserControl
{
    public NetworkPeerProvisioningView()
    {
        this.DataContext = new NetworkPeerProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPeerProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}