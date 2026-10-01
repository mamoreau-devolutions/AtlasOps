namespace AtlasOps.Features.Mobile.MobileFleetMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileFleetMonitoringView : UserControl
{
    public MobileFleetMonitoringView()
    {
        this.DataContext = new MobileFleetMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileFleetMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}