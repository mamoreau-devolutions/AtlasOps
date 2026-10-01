namespace AtlasOps.Features.Network.NetworkFirewallOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkFirewallOptimizationView : UserControl
{
    public NetworkFirewallOptimizationView()
    {
        this.DataContext = new NetworkFirewallOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkFirewallOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}