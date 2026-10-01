namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ContinuityPlanOptimizationView : UserControl
{
    public ContinuityPlanOptimizationView()
    {
        this.DataContext = new ContinuityPlanOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ContinuityPlanOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}