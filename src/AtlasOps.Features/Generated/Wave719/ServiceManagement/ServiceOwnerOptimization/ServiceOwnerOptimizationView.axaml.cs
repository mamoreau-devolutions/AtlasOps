namespace AtlasOps.Features.ServiceManagement.ServiceOwnerOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceOwnerOptimizationView : UserControl
{
    public ServiceOwnerOptimizationView()
    {
        this.DataContext = new ServiceOwnerOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceOwnerOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}