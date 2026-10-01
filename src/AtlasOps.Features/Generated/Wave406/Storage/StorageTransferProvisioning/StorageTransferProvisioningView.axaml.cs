namespace AtlasOps.Features.Storage.StorageTransferProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageTransferProvisioningView : UserControl
{
    public StorageTransferProvisioningView()
    {
        this.DataContext = new StorageTransferProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageTransferProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}