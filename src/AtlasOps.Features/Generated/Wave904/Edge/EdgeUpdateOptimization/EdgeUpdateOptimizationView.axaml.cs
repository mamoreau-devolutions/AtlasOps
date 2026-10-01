namespace AtlasOps.Features.Edge.EdgeUpdateOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeUpdateOptimizationView : UserControl
{
    public EdgeUpdateOptimizationView()
    {
        this.DataContext = new EdgeUpdateOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeUpdateOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}