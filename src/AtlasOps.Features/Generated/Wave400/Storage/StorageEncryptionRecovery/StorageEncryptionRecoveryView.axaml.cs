namespace AtlasOps.Features.Storage.StorageEncryptionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageEncryptionRecoveryView : UserControl
{
    public StorageEncryptionRecoveryView()
    {
        this.DataContext = new StorageEncryptionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageEncryptionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}