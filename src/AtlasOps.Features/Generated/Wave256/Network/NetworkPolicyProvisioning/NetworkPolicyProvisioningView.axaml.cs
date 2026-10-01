namespace AtlasOps.Features.Network.NetworkPolicyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPolicyProvisioningView : UserControl
{
    public NetworkPolicyProvisioningView()
    {
        this.DataContext = new NetworkPolicyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPolicyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}