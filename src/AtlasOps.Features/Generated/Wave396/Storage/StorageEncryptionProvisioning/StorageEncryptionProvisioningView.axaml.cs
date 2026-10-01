namespace AtlasOps.Features.Storage.StorageEncryptionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageEncryptionProvisioningView : UserControl
{
    public StorageEncryptionProvisioningView()
    {
        this.DataContext = new StorageEncryptionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageEncryptionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}