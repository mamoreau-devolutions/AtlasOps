namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryExerciseGovernanceView : UserControl
{
    public RecoveryExerciseGovernanceView()
    {
        this.DataContext = new RecoveryExerciseGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryExerciseGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}