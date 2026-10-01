namespace AtlasOps.Features.Edge.EdgeDeviceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeviceOptimizationView : UserControl
{
    public EdgeDeviceOptimizationView()
    {
        this.DataContext = new EdgeDeviceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeviceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}