namespace AtlasOps.Features.Cloud.CloudStorageMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudStorageMonitoringView : UserControl
{
    public CloudStorageMonitoringView()
    {
        this.DataContext = new CloudStorageMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudStorageMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}