namespace AtlasOps.Features.Storage.StorageReplicationProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageReplicationProvisioningView : UserControl
{
    public StorageReplicationProvisioningView()
    {
        this.DataContext = new StorageReplicationProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageReplicationProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}