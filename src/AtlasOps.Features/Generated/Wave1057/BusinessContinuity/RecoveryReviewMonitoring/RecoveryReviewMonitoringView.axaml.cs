namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryReviewMonitoringView : UserControl
{
    public RecoveryReviewMonitoringView()
    {
        this.DataContext = new RecoveryReviewMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryReviewMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}