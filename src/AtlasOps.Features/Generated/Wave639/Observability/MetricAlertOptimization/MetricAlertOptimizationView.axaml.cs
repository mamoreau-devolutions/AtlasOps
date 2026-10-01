namespace AtlasOps.Features.Observability.MetricAlertOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricAlertOptimizationView : UserControl
{
    public MetricAlertOptimizationView()
    {
        this.DataContext = new MetricAlertOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricAlertOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}