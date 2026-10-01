namespace AtlasOps.Features.Database.DatabaseMaintenanceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseMaintenanceOptimizationView : UserControl
{
    public DatabaseMaintenanceOptimizationView()
    {
        this.DataContext = new DatabaseMaintenanceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseMaintenanceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}