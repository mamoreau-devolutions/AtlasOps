namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryExerciseRecoveryView : UserControl
{
    public RecoveryExerciseRecoveryView()
    {
        this.DataContext = new RecoveryExerciseRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryExerciseRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}