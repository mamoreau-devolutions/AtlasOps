namespace AtlasOps.Features.Architecture.ArchitectureDependencyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDependencyOptimizationView : UserControl
{
    public ArchitectureDependencyOptimizationView()
    {
        this.DataContext = new ArchitectureDependencyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDependencyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}