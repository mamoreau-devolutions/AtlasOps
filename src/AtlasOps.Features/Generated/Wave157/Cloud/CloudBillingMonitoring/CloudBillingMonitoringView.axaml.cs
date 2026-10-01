namespace AtlasOps.Features.Cloud.CloudBillingMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudBillingMonitoringView : UserControl
{
    public CloudBillingMonitoringView()
    {
        this.DataContext = new CloudBillingMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudBillingMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}