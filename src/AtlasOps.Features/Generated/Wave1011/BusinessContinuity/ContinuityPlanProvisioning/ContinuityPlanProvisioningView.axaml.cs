namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ContinuityPlanProvisioningView : UserControl
{
    public ContinuityPlanProvisioningView()
    {
        this.DataContext = new ContinuityPlanProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ContinuityPlanProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}