namespace AtlasOps.Features.Compute.ComputeMetricOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeMetricOptimizationView : UserControl
{
    public ComputeMetricOptimizationView()
    {
        this.DataContext = new ComputeMetricOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeMetricOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}