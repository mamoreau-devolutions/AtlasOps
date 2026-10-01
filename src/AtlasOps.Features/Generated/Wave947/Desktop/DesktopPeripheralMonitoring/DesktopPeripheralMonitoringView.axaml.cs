namespace AtlasOps.Features.Desktop.DesktopPeripheralMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPeripheralMonitoringView : UserControl
{
    public DesktopPeripheralMonitoringView()
    {
        this.DataContext = new DesktopPeripheralMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPeripheralMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}