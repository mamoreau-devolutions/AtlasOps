namespace AtlasOps.Features.Cloud.CloudFunctionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudFunctionMonitoringView : UserControl
{
    public CloudFunctionMonitoringView()
    {
        this.DataContext = new CloudFunctionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudFunctionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}