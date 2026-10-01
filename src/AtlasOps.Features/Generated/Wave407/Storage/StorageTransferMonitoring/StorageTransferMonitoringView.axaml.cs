namespace AtlasOps.Features.Storage.StorageTransferMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageTransferMonitoringView : UserControl
{
    public StorageTransferMonitoringView()
    {
        this.DataContext = new StorageTransferMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageTransferMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}