namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MaintenanceWindowProvisioningView : UserControl
{
    public MaintenanceWindowProvisioningView()
    {
        this.DataContext = new MaintenanceWindowProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MaintenanceWindowProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}