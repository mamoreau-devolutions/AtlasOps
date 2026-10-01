namespace AtlasOps.Features.ServiceManagement.ServiceRequestProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceRequestProvisioningView : UserControl
{
    public ServiceRequestProvisioningView()
    {
        this.DataContext = new ServiceRequestProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceRequestProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}