namespace AtlasOps.Features.Network.NetworkRouteRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkRouteRecoveryView : UserControl
{
    public NetworkRouteRecoveryView()
    {
        this.DataContext = new NetworkRouteRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkRouteRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}