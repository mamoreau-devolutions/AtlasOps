namespace AtlasOps.Features.Observability.LogQueryMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogQueryMonitoringView : UserControl
{
    public LogQueryMonitoringView()
    {
        this.DataContext = new LogQueryMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogQueryMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}