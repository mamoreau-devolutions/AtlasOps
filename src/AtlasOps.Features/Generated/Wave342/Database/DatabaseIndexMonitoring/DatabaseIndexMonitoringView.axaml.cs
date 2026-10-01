namespace AtlasOps.Features.Database.DatabaseIndexMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseIndexMonitoringView : UserControl
{
    public DatabaseIndexMonitoringView()
    {
        this.DataContext = new DatabaseIndexMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseIndexMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}