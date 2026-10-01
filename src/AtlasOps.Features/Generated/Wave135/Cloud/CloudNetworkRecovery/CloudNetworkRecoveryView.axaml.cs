namespace AtlasOps.Features.Cloud.CloudNetworkRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudNetworkRecoveryView : UserControl
{
    public CloudNetworkRecoveryView()
    {
        this.DataContext = new CloudNetworkRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudNetworkRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}