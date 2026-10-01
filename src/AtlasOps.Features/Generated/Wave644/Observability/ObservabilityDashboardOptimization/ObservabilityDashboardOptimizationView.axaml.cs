namespace AtlasOps.Features.Observability.ObservabilityDashboardOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityDashboardOptimizationView : UserControl
{
    public ObservabilityDashboardOptimizationView()
    {
        this.DataContext = new ObservabilityDashboardOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityDashboardOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}