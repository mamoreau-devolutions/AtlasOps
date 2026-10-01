namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryExerciseMonitoringView : UserControl
{
    public RecoveryExerciseMonitoringView()
    {
        this.DataContext = new RecoveryExerciseMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryExerciseMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}