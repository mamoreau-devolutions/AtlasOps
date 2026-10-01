namespace AtlasOps.Features.FinOps.CostAllocationProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAllocationProvisioningView : UserControl
{
    public CostAllocationProvisioningView()
    {
        this.DataContext = new CostAllocationProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAllocationProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}