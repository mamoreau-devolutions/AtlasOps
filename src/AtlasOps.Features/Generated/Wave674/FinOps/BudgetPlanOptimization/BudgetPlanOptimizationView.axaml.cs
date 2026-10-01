namespace AtlasOps.Features.FinOps.BudgetPlanOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BudgetPlanOptimizationView : UserControl
{
    public BudgetPlanOptimizationView()
    {
        this.DataContext = new BudgetPlanOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BudgetPlanOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}