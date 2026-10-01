namespace AtlasOps.Features.Cloud.CloudNetworkProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudNetworkProvisioningView : UserControl
{
    public CloudNetworkProvisioningView()
    {
        this.DataContext = new CloudNetworkProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudNetworkProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}