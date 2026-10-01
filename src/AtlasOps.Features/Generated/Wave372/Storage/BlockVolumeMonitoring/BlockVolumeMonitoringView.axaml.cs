namespace AtlasOps.Features.Storage.BlockVolumeMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BlockVolumeMonitoringView : UserControl
{
    public BlockVolumeMonitoringView()
    {
        this.DataContext = new BlockVolumeMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BlockVolumeMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}