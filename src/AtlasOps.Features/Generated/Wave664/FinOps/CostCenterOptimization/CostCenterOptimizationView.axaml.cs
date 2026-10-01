namespace AtlasOps.Features.FinOps.CostCenterOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostCenterOptimizationView : UserControl
{
    public CostCenterOptimizationView()
    {
        this.DataContext = new CostCenterOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostCenterOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}