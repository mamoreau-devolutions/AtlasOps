namespace AtlasOps.Features.Storage.StorageQuotaProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageQuotaProvisioningView : UserControl
{
    public StorageQuotaProvisioningView()
    {
        this.DataContext = new StorageQuotaProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageQuotaProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}