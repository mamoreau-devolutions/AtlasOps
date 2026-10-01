namespace AtlasOps.Features.Desktop.DesktopProfileMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopProfileMonitoringView : UserControl
{
    public DesktopProfileMonitoringView()
    {
        this.DataContext = new DesktopProfileMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopProfileMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}