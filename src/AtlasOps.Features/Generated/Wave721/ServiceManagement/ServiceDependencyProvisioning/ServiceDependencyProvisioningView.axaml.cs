namespace AtlasOps.Features.ServiceManagement.ServiceDependencyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceDependencyProvisioningView : UserControl
{
    public ServiceDependencyProvisioningView()
    {
        this.DataContext = new ServiceDependencyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceDependencyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}