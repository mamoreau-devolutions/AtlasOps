namespace AtlasOps.Features.ServiceManagement.ServiceDependencyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceDependencyOptimizationView : UserControl
{
    public ServiceDependencyOptimizationView()
    {
        this.DataContext = new ServiceDependencyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceDependencyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}