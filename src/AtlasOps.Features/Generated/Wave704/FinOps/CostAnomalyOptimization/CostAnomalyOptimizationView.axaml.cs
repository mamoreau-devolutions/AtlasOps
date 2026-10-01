namespace AtlasOps.Features.FinOps.CostAnomalyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAnomalyOptimizationView : UserControl
{
    public CostAnomalyOptimizationView()
    {
        this.DataContext = new CostAnomalyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAnomalyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}