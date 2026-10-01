namespace AtlasOps.Features.Network.NetworkLoadBalancerProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkLoadBalancerProvisioningView : UserControl
{
    public NetworkLoadBalancerProvisioningView()
    {
        this.DataContext = new NetworkLoadBalancerProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkLoadBalancerProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}