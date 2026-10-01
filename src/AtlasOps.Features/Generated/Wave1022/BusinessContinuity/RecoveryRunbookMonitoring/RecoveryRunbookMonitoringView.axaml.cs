namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryRunbookMonitoringView : UserControl
{
    public RecoveryRunbookMonitoringView()
    {
        this.DataContext = new RecoveryRunbookMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryRunbookMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}