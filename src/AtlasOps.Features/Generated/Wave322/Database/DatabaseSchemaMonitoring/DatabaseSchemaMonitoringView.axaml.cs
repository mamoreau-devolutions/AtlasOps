namespace AtlasOps.Features.Database.DatabaseSchemaMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseSchemaMonitoringView : UserControl
{
    public DatabaseSchemaMonitoringView()
    {
        this.DataContext = new DatabaseSchemaMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseSchemaMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}