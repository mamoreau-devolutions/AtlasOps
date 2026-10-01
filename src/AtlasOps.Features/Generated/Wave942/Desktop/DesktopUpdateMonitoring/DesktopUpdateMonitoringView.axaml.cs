namespace AtlasOps.Features.Desktop.DesktopUpdateMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopUpdateMonitoringView : UserControl
{
    public DesktopUpdateMonitoringView()
    {
        this.DataContext = new DesktopUpdateMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopUpdateMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}