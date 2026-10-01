namespace AtlasOps.Features.Compute.ComputeLifecycleOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeLifecycleOptimizationView : UserControl
{
    public ComputeLifecycleOptimizationView()
    {
        this.DataContext = new ComputeLifecycleOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeLifecycleOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}