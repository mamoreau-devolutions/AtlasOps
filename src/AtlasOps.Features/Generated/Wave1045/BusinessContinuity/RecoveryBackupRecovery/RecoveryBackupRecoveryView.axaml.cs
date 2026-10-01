namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryBackupRecoveryView : UserControl
{
    public RecoveryBackupRecoveryView()
    {
        this.DataContext = new RecoveryBackupRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryBackupRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}