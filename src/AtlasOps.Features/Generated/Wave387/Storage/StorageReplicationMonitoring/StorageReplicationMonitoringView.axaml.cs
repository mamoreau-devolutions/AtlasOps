namespace AtlasOps.Features.Storage.StorageReplicationMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageReplicationMonitoringView : UserControl
{
    public StorageReplicationMonitoringView()
    {
        this.DataContext = new StorageReplicationMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageReplicationMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}