namespace AtlasOps.Features.Delivery.BuildArtifactOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildArtifactOptimizationView : UserControl
{
    public BuildArtifactOptimizationView()
    {
        this.DataContext = new BuildArtifactOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildArtifactOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}