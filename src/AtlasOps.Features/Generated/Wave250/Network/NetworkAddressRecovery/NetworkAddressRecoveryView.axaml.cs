namespace AtlasOps.Features.Network.NetworkAddressRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkAddressRecoveryView : UserControl
{
    public NetworkAddressRecoveryView()
    {
        this.DataContext = new NetworkAddressRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkAddressRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}