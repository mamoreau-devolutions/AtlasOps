namespace AtlasOps.Features.Desktop.DesktopPoolMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPoolMonitoringView : UserControl
{
    public DesktopPoolMonitoringView()
    {
        this.DataContext = new DesktopPoolMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPoolMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}