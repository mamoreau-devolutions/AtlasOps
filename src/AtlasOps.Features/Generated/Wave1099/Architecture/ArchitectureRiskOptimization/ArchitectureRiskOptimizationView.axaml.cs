namespace AtlasOps.Features.Architecture.ArchitectureRiskOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRiskOptimizationView : UserControl
{
    public ArchitectureRiskOptimizationView()
    {
        this.DataContext = new ArchitectureRiskOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRiskOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}