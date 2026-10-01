namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryBackupMonitoringView : UserControl
{
    public RecoveryBackupMonitoringView()
    {
        this.DataContext = new RecoveryBackupMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryBackupMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}