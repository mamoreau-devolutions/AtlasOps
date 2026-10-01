namespace AtlasOps.Features.Architecture.ArchitectureDecisionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDecisionOptimizationView : UserControl
{
    public ArchitectureDecisionOptimizationView()
    {
        this.DataContext = new ArchitectureDecisionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDecisionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}