namespace AtlasOps.Features.Storage.StorageSnapshotProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageSnapshotProvisioningView : UserControl
{
    public StorageSnapshotProvisioningView()
    {
        this.DataContext = new StorageSnapshotProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageSnapshotProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}