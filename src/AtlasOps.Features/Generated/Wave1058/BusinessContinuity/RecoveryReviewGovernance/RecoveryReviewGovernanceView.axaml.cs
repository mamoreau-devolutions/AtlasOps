namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryReviewGovernanceView : UserControl
{
    public RecoveryReviewGovernanceView()
    {
        this.DataContext = new RecoveryReviewGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryReviewGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}