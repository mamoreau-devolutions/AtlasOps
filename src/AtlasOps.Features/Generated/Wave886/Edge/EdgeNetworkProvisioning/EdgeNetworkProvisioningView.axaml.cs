namespace AtlasOps.Features.Edge.EdgeNetworkProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeNetworkProvisioningView : UserControl
{
    public EdgeNetworkProvisioningView()
    {
        this.DataContext = new EdgeNetworkProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeNetworkProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}