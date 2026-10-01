namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryFailoverMonitoringView : UserControl
{
    public RecoveryFailoverMonitoringView()
    {
        this.DataContext = new RecoveryFailoverMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryFailoverMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}