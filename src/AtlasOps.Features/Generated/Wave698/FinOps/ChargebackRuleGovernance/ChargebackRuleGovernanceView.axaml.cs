namespace AtlasOps.Features.FinOps.ChargebackRuleGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChargebackRuleGovernanceView : UserControl
{
    public ChargebackRuleGovernanceView()
    {
        this.DataContext = new ChargebackRuleGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChargebackRuleGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}