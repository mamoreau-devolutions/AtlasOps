namespace AtlasOps.Features.Storage.StorageReplicationRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageReplicationRecoveryView : UserControl
{
    public StorageReplicationRecoveryView()
    {
        this.DataContext = new StorageReplicationRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageReplicationRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}