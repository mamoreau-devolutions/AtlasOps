namespace AtlasOps.Features.Delivery.ReleaseCalendarMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseCalendarMonitoringView : UserControl
{
    public ReleaseCalendarMonitoringView()
    {
        this.DataContext = new ReleaseCalendarMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseCalendarMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}