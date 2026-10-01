namespace AtlasOps.Features.Database.DatabaseRestoreMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseRestoreMonitoringView : UserControl
{
    public DatabaseRestoreMonitoringView()
    {
        this.DataContext = new DatabaseRestoreMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseRestoreMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}