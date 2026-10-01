namespace AtlasOps.Features.Delivery.ReleaseMetricOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseMetricOptimizationView : UserControl
{
    public ReleaseMetricOptimizationView()
    {
        this.DataContext = new ReleaseMetricOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseMetricOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}