namespace AtlasOps.Features.Network.NetworkProbeProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkProbeProvisioningView : UserControl
{
    public NetworkProbeProvisioningView()
    {
        this.DataContext = new NetworkProbeProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkProbeProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}