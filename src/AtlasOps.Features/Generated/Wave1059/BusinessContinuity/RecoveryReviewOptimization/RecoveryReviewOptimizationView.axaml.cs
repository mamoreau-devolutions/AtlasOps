namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryReviewOptimizationView : UserControl
{
    public RecoveryReviewOptimizationView()
    {
        this.DataContext = new RecoveryReviewOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryReviewOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}