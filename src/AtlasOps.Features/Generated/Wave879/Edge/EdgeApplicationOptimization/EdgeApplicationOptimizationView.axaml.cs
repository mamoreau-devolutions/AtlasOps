namespace AtlasOps.Features.Edge.EdgeApplicationOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeApplicationOptimizationView : UserControl
{
    public EdgeApplicationOptimizationView()
    {
        this.DataContext = new EdgeApplicationOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeApplicationOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}