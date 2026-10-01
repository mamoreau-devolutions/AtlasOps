namespace AtlasOps.Features.FinOps.ChargebackRuleRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChargebackRuleRecoveryView : UserControl
{
    public ChargebackRuleRecoveryView()
    {
        this.DataContext = new ChargebackRuleRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChargebackRuleRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}