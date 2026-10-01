namespace AtlasOps.Features.Network.NetworkLoadBalancerMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkLoadBalancerMonitoringView : UserControl
{
    public NetworkLoadBalancerMonitoringView()
    {
        this.DataContext = new NetworkLoadBalancerMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkLoadBalancerMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}