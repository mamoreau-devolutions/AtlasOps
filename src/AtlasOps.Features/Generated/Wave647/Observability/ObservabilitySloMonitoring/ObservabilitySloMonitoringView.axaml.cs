namespace AtlasOps.Features.Observability.ObservabilitySloMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilitySloMonitoringView : UserControl
{
    public ObservabilitySloMonitoringView()
    {
        this.DataContext = new ObservabilitySloMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilitySloMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}