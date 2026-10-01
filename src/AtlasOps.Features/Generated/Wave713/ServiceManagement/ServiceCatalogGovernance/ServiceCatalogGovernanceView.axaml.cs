namespace AtlasOps.Features.ServiceManagement.ServiceCatalogGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceCatalogGovernanceView : UserControl
{
    public ServiceCatalogGovernanceView()
    {
        this.DataContext = new ServiceCatalogGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceCatalogGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}