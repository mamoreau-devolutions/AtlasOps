namespace AtlasOps.Features.Observability.ObservabilityRetentionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityRetentionMonitoringView : UserControl
{
    public ObservabilityRetentionMonitoringView()
    {
        this.DataContext = new ObservabilityRetentionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityRetentionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}