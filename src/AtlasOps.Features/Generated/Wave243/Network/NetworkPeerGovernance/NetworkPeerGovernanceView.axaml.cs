namespace AtlasOps.Features.Network.NetworkPeerGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPeerGovernanceView : UserControl
{
    public NetworkPeerGovernanceView()
    {
        this.DataContext = new NetworkPeerGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPeerGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}