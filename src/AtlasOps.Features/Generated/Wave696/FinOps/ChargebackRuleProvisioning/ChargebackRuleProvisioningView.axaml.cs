namespace AtlasOps.Features.FinOps.ChargebackRuleProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChargebackRuleProvisioningView : UserControl
{
    public ChargebackRuleProvisioningView()
    {
        this.DataContext = new ChargebackRuleProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChargebackRuleProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}