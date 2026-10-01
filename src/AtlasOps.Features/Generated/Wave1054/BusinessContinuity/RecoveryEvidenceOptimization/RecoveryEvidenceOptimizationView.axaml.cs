namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryEvidenceOptimizationView : UserControl
{
    public RecoveryEvidenceOptimizationView()
    {
        this.DataContext = new RecoveryEvidenceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryEvidenceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}