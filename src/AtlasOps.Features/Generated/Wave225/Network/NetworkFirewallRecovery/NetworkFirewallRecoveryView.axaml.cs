namespace AtlasOps.Features.Network.NetworkFirewallRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkFirewallRecoveryView : UserControl
{
    public NetworkFirewallRecoveryView()
    {
        this.DataContext = new NetworkFirewallRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkFirewallRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}