namespace AtlasOps.Features.Incidents.AlertRule;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AlertRuleView : UserControl
{
    public AlertRuleView()
    {
        this.DataContext = new AlertRuleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AlertRuleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}