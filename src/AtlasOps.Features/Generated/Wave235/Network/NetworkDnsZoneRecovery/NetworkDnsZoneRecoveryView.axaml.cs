namespace AtlasOps.Features.Network.NetworkDnsZoneRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkDnsZoneRecoveryView : UserControl
{
    public NetworkDnsZoneRecoveryView()
    {
        this.DataContext = new NetworkDnsZoneRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkDnsZoneRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}