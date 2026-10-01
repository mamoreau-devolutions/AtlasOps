namespace AtlasOps.Features.Database.NoSqlDatabaseMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NoSqlDatabaseMonitoringView : UserControl
{
    public NoSqlDatabaseMonitoringView()
    {
        this.DataContext = new NoSqlDatabaseMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NoSqlDatabaseMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}