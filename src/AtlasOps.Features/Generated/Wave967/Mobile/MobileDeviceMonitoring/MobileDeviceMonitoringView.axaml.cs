namespace AtlasOps.Features.Mobile.MobileDeviceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileDeviceMonitoringView : UserControl
{
    public MobileDeviceMonitoringView()
    {
        this.DataContext = new MobileDeviceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileDeviceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}