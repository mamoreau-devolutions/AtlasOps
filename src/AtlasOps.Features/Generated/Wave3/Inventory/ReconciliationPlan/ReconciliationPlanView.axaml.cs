namespace AtlasOps.Features.Inventory.ReconciliationPlan;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReconciliationPlanView : UserControl
{
    public ReconciliationPlanView()
    {
        this.DataContext = new ReconciliationPlanViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReconciliationPlanViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}