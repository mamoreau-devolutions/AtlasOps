namespace AtlasOps.Features.ServiceManagement.ServiceCatalogProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceCatalogProvisioningView : UserControl
{
    public ServiceCatalogProvisioningView()
    {
        this.DataContext = new ServiceCatalogProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceCatalogProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}