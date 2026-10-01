namespace AtlasOps.Features.Delivery.BuildPipelineOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildPipelineOptimizationView : UserControl
{
    public BuildPipelineOptimizationView()
    {
        this.DataContext = new BuildPipelineOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildPipelineOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}