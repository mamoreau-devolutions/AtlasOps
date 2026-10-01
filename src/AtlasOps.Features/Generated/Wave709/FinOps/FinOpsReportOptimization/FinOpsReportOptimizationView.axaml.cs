namespace AtlasOps.Features.FinOps.FinOpsReportOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FinOpsReportOptimizationView : UserControl
{
    public FinOpsReportOptimizationView()
    {
        this.DataContext = new FinOpsReportOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FinOpsReportOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}