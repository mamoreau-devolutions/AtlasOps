namespace AtlasOps.Features.Database.DatabaseBackupRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseBackupRecoveryView : UserControl
{
    public DatabaseBackupRecoveryView()
    {
        this.DataContext = new DatabaseBackupRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseBackupRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}