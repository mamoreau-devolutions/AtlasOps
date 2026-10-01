namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryBackupOptimizationView : UserControl
{
    public RecoveryBackupOptimizationView()
    {
        this.DataContext = new RecoveryBackupOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryBackupOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}