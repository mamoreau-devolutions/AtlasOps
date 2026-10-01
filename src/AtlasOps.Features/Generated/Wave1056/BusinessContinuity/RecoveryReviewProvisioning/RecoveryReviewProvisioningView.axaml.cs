namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryReviewProvisioningView : UserControl
{
    public RecoveryReviewProvisioningView()
    {
        this.DataContext = new RecoveryReviewProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryReviewProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}