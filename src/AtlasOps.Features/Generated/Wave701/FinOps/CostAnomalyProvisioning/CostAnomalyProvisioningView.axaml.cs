namespace AtlasOps.Features.FinOps.CostAnomalyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAnomalyProvisioningView : UserControl
{
    public CostAnomalyProvisioningView()
    {
        this.DataContext = new CostAnomalyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAnomalyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}