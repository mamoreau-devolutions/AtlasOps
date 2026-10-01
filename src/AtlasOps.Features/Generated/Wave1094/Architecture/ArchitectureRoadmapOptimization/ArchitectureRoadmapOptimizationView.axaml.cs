namespace AtlasOps.Features.Architecture.ArchitectureRoadmapOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRoadmapOptimizationView : UserControl
{
    public ArchitectureRoadmapOptimizationView()
    {
        this.DataContext = new ArchitectureRoadmapOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRoadmapOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}