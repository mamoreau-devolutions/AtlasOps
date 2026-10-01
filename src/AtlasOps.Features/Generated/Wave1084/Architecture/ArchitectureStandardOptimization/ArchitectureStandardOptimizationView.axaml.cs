namespace AtlasOps.Features.Architecture.ArchitectureStandardOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureStandardOptimizationView : UserControl
{
    public ArchitectureStandardOptimizationView()
    {
        this.DataContext = new ArchitectureStandardOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureStandardOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}