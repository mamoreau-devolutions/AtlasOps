namespace AtlasOps.Features.Network.NetworkRouteProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkRouteProvisioningView : UserControl
{
    public NetworkRouteProvisioningView()
    {
        this.DataContext = new NetworkRouteProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkRouteProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}