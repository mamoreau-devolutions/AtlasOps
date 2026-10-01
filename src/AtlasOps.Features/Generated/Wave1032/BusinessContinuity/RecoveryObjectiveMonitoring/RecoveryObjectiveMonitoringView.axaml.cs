namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryObjectiveMonitoringView : UserControl
{
    public RecoveryObjectiveMonitoringView()
    {
        this.DataContext = new RecoveryObjectiveMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryObjectiveMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}