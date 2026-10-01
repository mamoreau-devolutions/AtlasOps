namespace AtlasOps.Features.Network.NetworkPeerRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPeerRecoveryView : UserControl
{
    public NetworkPeerRecoveryView()
    {
        this.DataContext = new NetworkPeerRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPeerRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}