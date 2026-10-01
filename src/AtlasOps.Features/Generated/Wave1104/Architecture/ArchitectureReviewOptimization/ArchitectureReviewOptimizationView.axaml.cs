namespace AtlasOps.Features.Architecture.ArchitectureReviewOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureReviewOptimizationView : UserControl
{
    public ArchitectureReviewOptimizationView()
    {
        this.DataContext = new ArchitectureReviewOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureReviewOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}