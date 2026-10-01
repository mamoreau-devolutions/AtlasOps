namespace AtlasOps.Features.Analytics.MetricFormula;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricFormulaView : UserControl
{
    public MetricFormulaView()
    {
        this.DataContext = new MetricFormulaViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricFormulaViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}