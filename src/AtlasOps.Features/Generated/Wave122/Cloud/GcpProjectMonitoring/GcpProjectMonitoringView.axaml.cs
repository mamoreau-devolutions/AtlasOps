namespace AtlasOps.Features.Cloud.GcpProjectMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class GcpProjectMonitoringView : UserControl
{
    public GcpProjectMonitoringView()
    {
        this.DataContext = new GcpProjectMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is GcpProjectMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}