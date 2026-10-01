namespace AtlasOps.Features.Desktop.DesktopImageMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopImageMonitoringView : UserControl
{
    public DesktopImageMonitoringView()
    {
        this.DataContext = new DesktopImageMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopImageMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}