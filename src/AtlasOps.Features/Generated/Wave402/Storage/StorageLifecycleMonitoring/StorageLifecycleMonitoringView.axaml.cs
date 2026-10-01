namespace AtlasOps.Features.Storage.StorageLifecycleMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageLifecycleMonitoringView : UserControl
{
    public StorageLifecycleMonitoringView()
    {
        this.DataContext = new StorageLifecycleMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageLifecycleMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}