namespace AtlasOps.Features.Edge.EdgeIncidentOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeIncidentOptimizationView : UserControl
{
    public EdgeIncidentOptimizationView()
    {
        this.DataContext = new EdgeIncidentOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeIncidentOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}