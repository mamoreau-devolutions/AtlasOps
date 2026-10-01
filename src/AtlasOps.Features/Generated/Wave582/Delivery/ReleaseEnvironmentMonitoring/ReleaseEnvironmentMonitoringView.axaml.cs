namespace AtlasOps.Features.Delivery.ReleaseEnvironmentMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseEnvironmentMonitoringView : UserControl
{
    public ReleaseEnvironmentMonitoringView()
    {
        this.DataContext = new ReleaseEnvironmentMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseEnvironmentMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}