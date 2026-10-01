namespace AtlasOps.Features.Architecture.ArchitectureComponentOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureComponentOptimizationView : UserControl
{
    public ArchitectureComponentOptimizationView()
    {
        this.DataContext = new ArchitectureComponentOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureComponentOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}