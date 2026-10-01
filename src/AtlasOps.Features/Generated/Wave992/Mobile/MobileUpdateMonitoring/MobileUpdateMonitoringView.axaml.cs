namespace AtlasOps.Features.Mobile.MobileUpdateMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileUpdateMonitoringView : UserControl
{
    public MobileUpdateMonitoringView()
    {
        this.DataContext = new MobileUpdateMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileUpdateMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}