namespace AtlasOps.Features.Cloud.CloudIdentityMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudIdentityMonitoringView : UserControl
{
    public CloudIdentityMonitoringView()
    {
        this.DataContext = new CloudIdentityMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudIdentityMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}