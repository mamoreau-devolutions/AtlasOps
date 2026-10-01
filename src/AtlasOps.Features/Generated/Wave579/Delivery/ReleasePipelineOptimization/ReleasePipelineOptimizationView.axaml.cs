namespace AtlasOps.Features.Delivery.ReleasePipelineOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleasePipelineOptimizationView : UserControl
{
    public ReleasePipelineOptimizationView()
    {
        this.DataContext = new ReleasePipelineOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleasePipelineOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}