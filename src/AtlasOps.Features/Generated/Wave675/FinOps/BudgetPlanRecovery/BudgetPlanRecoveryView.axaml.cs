namespace AtlasOps.Features.FinOps.BudgetPlanRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BudgetPlanRecoveryView : UserControl
{
    public BudgetPlanRecoveryView()
    {
        this.DataContext = new BudgetPlanRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BudgetPlanRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}