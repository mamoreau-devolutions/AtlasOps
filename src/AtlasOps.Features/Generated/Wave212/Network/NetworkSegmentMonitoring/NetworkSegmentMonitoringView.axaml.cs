namespace AtlasOps.Features.Network.NetworkSegmentMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkSegmentMonitoringView : UserControl
{
    public NetworkSegmentMonitoringView()
    {
        this.DataContext = new NetworkSegmentMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkSegmentMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}