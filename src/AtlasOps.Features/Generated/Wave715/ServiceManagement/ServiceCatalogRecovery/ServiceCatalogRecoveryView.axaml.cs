namespace AtlasOps.Features.ServiceManagement.ServiceCatalogRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceCatalogRecoveryView : UserControl
{
    public ServiceCatalogRecoveryView()
    {
        this.DataContext = new ServiceCatalogRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceCatalogRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}