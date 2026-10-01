namespace AtlasOps.Features.Edge.EdgeDeviceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeviceMonitoringView : UserControl
{
    public EdgeDeviceMonitoringView()
    {
        this.DataContext = new EdgeDeviceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeviceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}