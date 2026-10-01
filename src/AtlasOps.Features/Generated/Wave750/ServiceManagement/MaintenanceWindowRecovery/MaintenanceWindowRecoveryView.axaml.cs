namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MaintenanceWindowRecoveryView : UserControl
{
    public MaintenanceWindowRecoveryView()
    {
        this.DataContext = new MaintenanceWindowRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MaintenanceWindowRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}