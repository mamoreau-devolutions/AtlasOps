namespace AtlasOps.Features.ServiceManagement.ServiceCatalogOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceCatalogOptimizationView : UserControl
{
    public ServiceCatalogOptimizationView()
    {
        this.DataContext = new ServiceCatalogOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceCatalogOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}