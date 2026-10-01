namespace AtlasOps.Features.Database.DatabaseBackupMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseBackupMonitoringView : UserControl
{
    public DatabaseBackupMonitoringView()
    {
        this.DataContext = new DatabaseBackupMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseBackupMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}