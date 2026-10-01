namespace AtlasOps.Features.Architecture.ArchitectureExceptionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureExceptionOptimizationView : UserControl
{
    public ArchitectureExceptionOptimizationView()
    {
        this.DataContext = new ArchitectureExceptionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureExceptionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}