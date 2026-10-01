namespace AtlasOps.Features.Network.NetworkLoadBalancerRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkLoadBalancerRecoveryView : UserControl
{
    public NetworkLoadBalancerRecoveryView()
    {
        this.DataContext = new NetworkLoadBalancerRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkLoadBalancerRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}