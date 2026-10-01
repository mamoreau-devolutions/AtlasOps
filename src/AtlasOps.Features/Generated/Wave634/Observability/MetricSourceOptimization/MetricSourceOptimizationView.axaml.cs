namespace AtlasOps.Features.Observability.MetricSourceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricSourceOptimizationView : UserControl
{
    public MetricSourceOptimizationView()
    {
        this.DataContext = new MetricSourceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricSourceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}