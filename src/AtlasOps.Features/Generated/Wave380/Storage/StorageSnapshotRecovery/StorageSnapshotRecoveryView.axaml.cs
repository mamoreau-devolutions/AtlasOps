namespace AtlasOps.Features.Storage.StorageSnapshotRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageSnapshotRecoveryView : UserControl
{
    public StorageSnapshotRecoveryView()
    {
        this.DataContext = new StorageSnapshotRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageSnapshotRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}