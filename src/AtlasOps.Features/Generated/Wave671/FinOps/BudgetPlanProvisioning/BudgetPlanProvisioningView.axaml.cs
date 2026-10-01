namespace AtlasOps.Features.FinOps.BudgetPlanProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BudgetPlanProvisioningView : UserControl
{
    public BudgetPlanProvisioningView()
    {
        this.DataContext = new BudgetPlanProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BudgetPlanProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}