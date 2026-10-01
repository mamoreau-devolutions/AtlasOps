namespace AtlasOps.Features.Mobile.MobileComplianceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileComplianceProvisioningView : UserControl
{
    public MobileComplianceProvisioningView()
    {
        this.DataContext = new MobileComplianceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileComplianceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}