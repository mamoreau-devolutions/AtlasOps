namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryDependencyMonitoringView : UserControl
{
    public RecoveryDependencyMonitoringView()
    {
        this.DataContext = new RecoveryDependencyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryDependencyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}