namespace AtlasOps.Features.Compute.ComputeScaleSetOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScaleSetOptimizationView : UserControl
{
    public ComputeScaleSetOptimizationView()
    {
        this.DataContext = new ComputeScaleSetOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScaleSetOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}