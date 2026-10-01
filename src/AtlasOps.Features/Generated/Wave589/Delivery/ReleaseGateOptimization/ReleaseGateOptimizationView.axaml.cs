namespace AtlasOps.Features.Delivery.ReleaseGateOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseGateOptimizationView : UserControl
{
    public ReleaseGateOptimizationView()
    {
        this.DataContext = new ReleaseGateOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseGateOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}