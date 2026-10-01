namespace AtlasOps.Features.Connections.GatewayRouting;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class GatewayRoutingView : UserControl
{
    public GatewayRoutingView()
    {
        this.DataContext = new GatewayRoutingViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is GatewayRoutingViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}