namespace AtlasOps.Features.FinOps.CostAllocationOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAllocationOptimizationView : UserControl
{
    public CostAllocationOptimizationView()
    {
        this.DataContext = new CostAllocationOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAllocationOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}