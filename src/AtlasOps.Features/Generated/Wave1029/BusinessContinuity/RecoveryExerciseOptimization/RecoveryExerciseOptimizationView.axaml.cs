namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryExerciseOptimizationView : UserControl
{
    public RecoveryExerciseOptimizationView()
    {
        this.DataContext = new RecoveryExerciseOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryExerciseOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}