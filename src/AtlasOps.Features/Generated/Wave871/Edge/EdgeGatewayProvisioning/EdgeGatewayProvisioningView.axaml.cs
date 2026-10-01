namespace AtlasOps.Features.Edge.EdgeGatewayProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeGatewayProvisioningView : UserControl
{
    public EdgeGatewayProvisioningView()
    {
        this.DataContext = new EdgeGatewayProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeGatewayProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}