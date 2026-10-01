namespace AtlasOps.Features.Edge.EdgeNetworkOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeNetworkOptimizationView : UserControl
{
    public EdgeNetworkOptimizationView()
    {
        this.DataContext = new EdgeNetworkOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeNetworkOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}