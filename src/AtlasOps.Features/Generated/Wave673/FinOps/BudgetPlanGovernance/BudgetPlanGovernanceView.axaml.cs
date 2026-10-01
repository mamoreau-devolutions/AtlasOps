namespace AtlasOps.Features.FinOps.BudgetPlanGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BudgetPlanGovernanceView : UserControl
{
    public BudgetPlanGovernanceView()
    {
        this.DataContext = new BudgetPlanGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BudgetPlanGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}