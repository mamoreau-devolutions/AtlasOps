namespace AtlasOps.Features.Database.DatabaseBackupProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseBackupProvisioningView : UserControl
{
    public DatabaseBackupProvisioningView()
    {
        this.DataContext = new DatabaseBackupProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseBackupProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}