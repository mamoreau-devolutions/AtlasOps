namespace AtlasOps.Features.FinOps.ChargebackRuleOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChargebackRuleOptimizationView : UserControl
{
    public ChargebackRuleOptimizationView()
    {
        this.DataContext = new ChargebackRuleOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChargebackRuleOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}