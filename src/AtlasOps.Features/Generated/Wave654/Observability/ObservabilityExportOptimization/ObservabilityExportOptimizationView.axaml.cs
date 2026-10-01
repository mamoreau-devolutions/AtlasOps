namespace AtlasOps.Features.Observability.ObservabilityExportOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityExportOptimizationView : UserControl
{
    public ObservabilityExportOptimizationView()
    {
        this.DataContext = new ObservabilityExportOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityExportOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}