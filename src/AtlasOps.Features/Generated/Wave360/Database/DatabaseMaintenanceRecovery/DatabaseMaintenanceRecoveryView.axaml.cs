namespace AtlasOps.Features.Database.DatabaseMaintenanceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseMaintenanceRecoveryView : UserControl
{
    public DatabaseMaintenanceRecoveryView()
    {
        this.DataContext = new DatabaseMaintenanceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseMaintenanceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}