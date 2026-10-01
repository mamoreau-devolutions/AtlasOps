namespace AtlasOps.Features.FinOps.BudgetPlanMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BudgetPlanMonitoringView : UserControl
{
    public BudgetPlanMonitoringView()
    {
        this.DataContext = new BudgetPlanMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BudgetPlanMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}