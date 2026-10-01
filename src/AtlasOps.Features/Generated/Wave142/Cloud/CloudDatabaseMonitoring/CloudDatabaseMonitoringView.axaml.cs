namespace AtlasOps.Features.Cloud.CloudDatabaseMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudDatabaseMonitoringView : UserControl
{
    public CloudDatabaseMonitoringView()
    {
        this.DataContext = new CloudDatabaseMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudDatabaseMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}