namespace AtlasOps.Features.Delivery.ReleaseRollbackMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseRollbackMonitoringView : UserControl
{
    public ReleaseRollbackMonitoringView()
    {
        this.DataContext = new ReleaseRollbackMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseRollbackMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}