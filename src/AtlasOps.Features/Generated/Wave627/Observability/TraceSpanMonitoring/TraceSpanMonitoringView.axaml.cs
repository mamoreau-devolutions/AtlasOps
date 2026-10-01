namespace AtlasOps.Features.Observability.TraceSpanMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TraceSpanMonitoringView : UserControl
{
    public TraceSpanMonitoringView()
    {
        this.DataContext = new TraceSpanMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TraceSpanMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}