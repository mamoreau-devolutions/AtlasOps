namespace AtlasOps.Features.Cloud.CloudRegionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudRegionMonitoringView : UserControl
{
    public CloudRegionMonitoringView()
    {
        this.DataContext = new CloudRegionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudRegionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}