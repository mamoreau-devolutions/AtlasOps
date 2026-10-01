namespace AtlasOps.Features.Cloud.CloudStorageProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudStorageProvisioningView : UserControl
{
    public CloudStorageProvisioningView()
    {
        this.DataContext = new CloudStorageProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudStorageProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}