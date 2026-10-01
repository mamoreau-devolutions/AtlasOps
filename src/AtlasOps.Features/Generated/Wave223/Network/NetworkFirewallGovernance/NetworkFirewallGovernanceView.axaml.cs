namespace AtlasOps.Features.Network.NetworkFirewallGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkFirewallGovernanceView : UserControl
{
    public NetworkFirewallGovernanceView()
    {
        this.DataContext = new NetworkFirewallGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkFirewallGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}