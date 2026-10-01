namespace AtlasOps.Features.Database.DatabaseMaintenanceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseMaintenanceProvisioningView : UserControl
{
    public DatabaseMaintenanceProvisioningView()
    {
        this.DataContext = new DatabaseMaintenanceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseMaintenanceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}