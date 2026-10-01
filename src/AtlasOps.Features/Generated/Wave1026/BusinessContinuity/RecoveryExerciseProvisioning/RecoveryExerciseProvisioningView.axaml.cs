namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryExerciseProvisioningView : UserControl
{
    public RecoveryExerciseProvisioningView()
    {
        this.DataContext = new RecoveryExerciseProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryExerciseProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}