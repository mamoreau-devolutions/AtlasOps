namespace AtlasOps.Features.Desktop.DesktopApplicationMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopApplicationMonitoringView : UserControl
{
    public DesktopApplicationMonitoringView()
    {
        this.DataContext = new DesktopApplicationMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopApplicationMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}