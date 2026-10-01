namespace AtlasOps.Features.Database.SqlDatabaseMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SqlDatabaseMonitoringView : UserControl
{
    public SqlDatabaseMonitoringView()
    {
        this.DataContext = new SqlDatabaseMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SqlDatabaseMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}