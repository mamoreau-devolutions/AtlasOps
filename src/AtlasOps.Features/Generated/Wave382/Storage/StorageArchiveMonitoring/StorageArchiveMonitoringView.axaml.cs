namespace AtlasOps.Features.Storage.StorageArchiveMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageArchiveMonitoringView : UserControl
{
    public StorageArchiveMonitoringView()
    {
        this.DataContext = new StorageArchiveMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageArchiveMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}