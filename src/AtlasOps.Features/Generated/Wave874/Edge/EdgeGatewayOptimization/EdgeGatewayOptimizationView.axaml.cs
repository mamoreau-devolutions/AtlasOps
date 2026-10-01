namespace AtlasOps.Features.Edge.EdgeGatewayOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeGatewayOptimizationView : UserControl
{
    public EdgeGatewayOptimizationView()
    {
        this.DataContext = new EdgeGatewayOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeGatewayOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}