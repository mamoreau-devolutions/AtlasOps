namespace AtlasOps.Features.FinOps.SavingsPlanOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SavingsPlanOptimizationView : UserControl
{
    public SavingsPlanOptimizationView()
    {
        this.DataContext = new SavingsPlanOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SavingsPlanOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}