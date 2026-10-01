namespace AtlasOps.Features.Network.NetworkLoadBalancerGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkLoadBalancerGovernanceView : UserControl
{
    public NetworkLoadBalancerGovernanceView()
    {
        this.DataContext = new NetworkLoadBalancerGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkLoadBalancerGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}