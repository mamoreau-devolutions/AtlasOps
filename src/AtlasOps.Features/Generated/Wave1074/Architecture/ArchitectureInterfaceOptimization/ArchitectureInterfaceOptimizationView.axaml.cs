namespace AtlasOps.Features.Architecture.ArchitectureInterfaceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureInterfaceOptimizationView : UserControl
{
    public ArchitectureInterfaceOptimizationView()
    {
        this.DataContext = new ArchitectureInterfaceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureInterfaceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}