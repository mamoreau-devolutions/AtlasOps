namespace AtlasOps.Features.Database.DatabaseBackupGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseBackupGovernanceView : UserControl
{
    public DatabaseBackupGovernanceView()
    {
        this.DataContext = new DatabaseBackupGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseBackupGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}