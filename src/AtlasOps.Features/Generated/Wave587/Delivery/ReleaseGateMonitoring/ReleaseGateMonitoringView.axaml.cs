namespace AtlasOps.Features.Delivery.ReleaseGateMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseGateMonitoringView : UserControl
{
    public ReleaseGateMonitoringView()
    {
        this.DataContext = new ReleaseGateMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseGateMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}