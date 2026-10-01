namespace AtlasOps.Features.Storage.StorageLifecycleProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageLifecycleProvisioningView : UserControl
{
    public StorageLifecycleProvisioningView()
    {
        this.DataContext = new StorageLifecycleProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageLifecycleProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}