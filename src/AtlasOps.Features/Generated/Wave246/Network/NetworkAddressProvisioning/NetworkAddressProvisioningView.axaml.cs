namespace AtlasOps.Features.Network.NetworkAddressProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkAddressProvisioningView : UserControl
{
    public NetworkAddressProvisioningView()
    {
        this.DataContext = new NetworkAddressProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkAddressProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}