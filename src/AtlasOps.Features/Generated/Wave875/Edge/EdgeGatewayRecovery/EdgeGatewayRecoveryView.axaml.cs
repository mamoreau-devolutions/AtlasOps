namespace AtlasOps.Features.Edge.EdgeGatewayRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeGatewayRecoveryView : UserControl
{
    public EdgeGatewayRecoveryView()
    {
        this.DataContext = new EdgeGatewayRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeGatewayRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}