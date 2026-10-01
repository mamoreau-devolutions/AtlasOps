namespace AtlasOps.Features.Observability.TraceSourceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TraceSourceMonitoringView : UserControl
{
    public TraceSourceMonitoringView()
    {
        this.DataContext = new TraceSourceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TraceSourceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}