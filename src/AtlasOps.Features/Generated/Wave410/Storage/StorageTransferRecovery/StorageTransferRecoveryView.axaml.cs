namespace AtlasOps.Features.Storage.StorageTransferRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageTransferRecoveryView : UserControl
{
    public StorageTransferRecoveryView()
    {
        this.DataContext = new StorageTransferRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageTransferRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}