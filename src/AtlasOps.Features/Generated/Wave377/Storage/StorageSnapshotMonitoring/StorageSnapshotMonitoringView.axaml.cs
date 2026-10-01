namespace AtlasOps.Features.Storage.StorageSnapshotMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageSnapshotMonitoringView : UserControl
{
    public StorageSnapshotMonitoringView()
    {
        this.DataContext = new StorageSnapshotMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageSnapshotMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}