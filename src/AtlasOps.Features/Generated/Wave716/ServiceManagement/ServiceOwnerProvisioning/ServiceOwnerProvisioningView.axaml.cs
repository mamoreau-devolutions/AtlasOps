namespace AtlasOps.Features.ServiceManagement.ServiceOwnerProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceOwnerProvisioningView : UserControl
{
    public ServiceOwnerProvisioningView()
    {
        this.DataContext = new ServiceOwnerProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceOwnerProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}