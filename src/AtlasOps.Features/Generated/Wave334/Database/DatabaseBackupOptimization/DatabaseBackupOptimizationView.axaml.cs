namespace AtlasOps.Features.Database.DatabaseBackupOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseBackupOptimizationView : UserControl
{
    public DatabaseBackupOptimizationView()
    {
        this.DataContext = new DatabaseBackupOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseBackupOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}