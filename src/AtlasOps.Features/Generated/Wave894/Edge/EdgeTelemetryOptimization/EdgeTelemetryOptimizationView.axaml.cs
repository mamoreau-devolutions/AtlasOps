namespace AtlasOps.Features.Edge.EdgeTelemetryOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeTelemetryOptimizationView : UserControl
{
    public EdgeTelemetryOptimizationView()
    {
        this.DataContext = new EdgeTelemetryOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeTelemetryOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}