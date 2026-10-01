namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MaintenanceWindowOptimizationView : UserControl
{
    public MaintenanceWindowOptimizationView()
    {
        this.DataContext = new MaintenanceWindowOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MaintenanceWindowOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}