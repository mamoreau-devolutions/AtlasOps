namespace AtlasOps.Features.ServiceManagement.ServiceRequestOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceRequestOptimizationView : UserControl
{
    public ServiceRequestOptimizationView()
    {
        this.DataContext = new ServiceRequestOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceRequestOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}