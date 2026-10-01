namespace AtlasOps.Features.Cloud.CloudRegionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudRegionProvisioningView : UserControl
{
    public CloudRegionProvisioningView()
    {
        this.DataContext = new CloudRegionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudRegionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}