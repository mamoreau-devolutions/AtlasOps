namespace AtlasOps.Features.Edge.EdgeSiteOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeSiteOptimizationView : UserControl
{
    public EdgeSiteOptimizationView()
    {
        this.DataContext = new EdgeSiteOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeSiteOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}