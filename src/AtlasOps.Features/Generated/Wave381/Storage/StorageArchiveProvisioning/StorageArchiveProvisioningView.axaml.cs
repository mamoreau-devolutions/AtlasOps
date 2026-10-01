namespace AtlasOps.Features.Storage.StorageArchiveProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageArchiveProvisioningView : UserControl
{
    public StorageArchiveProvisioningView()
    {
        this.DataContext = new StorageArchiveProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageArchiveProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}