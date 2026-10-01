namespace AtlasOps.Features.Database.DatabaseQueryMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseQueryMonitoringView : UserControl
{
    public DatabaseQueryMonitoringView()
    {
        this.DataContext = new DatabaseQueryMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseQueryMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}