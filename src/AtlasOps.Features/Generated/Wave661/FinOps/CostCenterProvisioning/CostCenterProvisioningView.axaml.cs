namespace AtlasOps.Features.FinOps.CostCenterProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostCenterProvisioningView : UserControl
{
    public CostCenterProvisioningView()
    {
        this.DataContext = new CostCenterProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostCenterProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}