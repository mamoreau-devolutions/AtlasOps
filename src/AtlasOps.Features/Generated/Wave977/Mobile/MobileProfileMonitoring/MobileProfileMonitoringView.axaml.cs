namespace AtlasOps.Features.Mobile.MobileProfileMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileProfileMonitoringView : UserControl
{
    public MobileProfileMonitoringView()
    {
        this.DataContext = new MobileProfileMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileProfileMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}