namespace AtlasOps.Features.Data.DataSourceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataSourceMonitoringView : UserControl
{
    public DataSourceMonitoringView()
    {
        this.DataContext = new DataSourceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataSourceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}