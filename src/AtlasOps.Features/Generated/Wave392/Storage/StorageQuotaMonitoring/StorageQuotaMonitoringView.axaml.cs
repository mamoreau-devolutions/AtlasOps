namespace AtlasOps.Features.Storage.StorageQuotaMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageQuotaMonitoringView : UserControl
{
    public StorageQuotaMonitoringView()
    {
        this.DataContext = new StorageQuotaMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageQuotaMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}