namespace AtlasOps.Features.Sync.SyncCheckpoint;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SyncCheckpointView : UserControl
{
    public SyncCheckpointView()
    {
        this.DataContext = new SyncCheckpointViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SyncCheckpointViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}