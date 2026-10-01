namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryEvidenceMonitoringView : UserControl
{
    public RecoveryEvidenceMonitoringView()
    {
        this.DataContext = new RecoveryEvidenceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryEvidenceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}