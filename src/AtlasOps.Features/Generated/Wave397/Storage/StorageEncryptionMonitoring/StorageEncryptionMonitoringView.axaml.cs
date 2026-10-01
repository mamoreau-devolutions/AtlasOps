namespace AtlasOps.Features.Storage.StorageEncryptionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageEncryptionMonitoringView : UserControl
{
    public StorageEncryptionMonitoringView()
    {
        this.DataContext = new StorageEncryptionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageEncryptionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}