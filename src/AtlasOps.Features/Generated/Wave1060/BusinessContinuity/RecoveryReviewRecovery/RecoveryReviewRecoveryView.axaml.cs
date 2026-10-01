namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryReviewRecoveryView : UserControl
{
    public RecoveryReviewRecoveryView()
    {
        this.DataContext = new RecoveryReviewRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryReviewRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}