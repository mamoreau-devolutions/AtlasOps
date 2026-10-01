namespace AtlasOps.Features.Automation.RollbackPlan;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RollbackPlanView : UserControl
{
    public RollbackPlanView()
    {
        this.DataContext = new RollbackPlanViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RollbackPlanViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}