namespace AtlasOps.Features.Database.DatabaseMaintenanceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseMaintenanceMonitoringView : UserControl
{
    public DatabaseMaintenanceMonitoringView()
    {
        this.DataContext = new DatabaseMaintenanceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseMaintenanceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}