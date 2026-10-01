namespace AtlasOps.Features.Network.NetworkLoadBalancerOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkLoadBalancerOptimizationView : UserControl
{
    public NetworkLoadBalancerOptimizationView()
    {
        this.DataContext = new NetworkLoadBalancerOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkLoadBalancerOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}