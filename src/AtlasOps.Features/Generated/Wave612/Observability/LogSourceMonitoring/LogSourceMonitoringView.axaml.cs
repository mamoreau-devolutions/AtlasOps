namespace AtlasOps.Features.Observability.LogSourceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogSourceMonitoringView : UserControl
{
    public LogSourceMonitoringView()
    {
        this.DataContext = new LogSourceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogSourceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}