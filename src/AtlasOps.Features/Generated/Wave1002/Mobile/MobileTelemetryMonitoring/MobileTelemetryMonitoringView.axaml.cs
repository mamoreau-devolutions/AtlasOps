namespace AtlasOps.Features.Mobile.MobileTelemetryMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileTelemetryMonitoringView : UserControl
{
    public MobileTelemetryMonitoringView()
    {
        this.DataContext = new MobileTelemetryMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileTelemetryMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}