namespace AtlasOps.Features.FinOps.ChargebackRuleMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChargebackRuleMonitoringView : UserControl
{
    public ChargebackRuleMonitoringView()
    {
        this.DataContext = new ChargebackRuleMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChargebackRuleMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}