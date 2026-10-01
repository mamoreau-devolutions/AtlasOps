namespace AtlasOps.Features.Desktop.DesktopPolicyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPolicyMonitoringView : UserControl
{
    public DesktopPolicyMonitoringView()
    {
        this.DataContext = new DesktopPolicyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPolicyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}