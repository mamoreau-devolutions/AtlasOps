namespace AtlasOps.Features.Architecture.ArchitectureEvidenceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureEvidenceOptimizationView : UserControl
{
    public ArchitectureEvidenceOptimizationView()
    {
        this.DataContext = new ArchitectureEvidenceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureEvidenceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}