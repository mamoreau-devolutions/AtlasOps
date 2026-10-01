namespace AtlasOps.Features.Mobile.MobileSupportMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileSupportMonitoringView : UserControl
{
    public MobileSupportMonitoringView()
    {
        this.DataContext = new MobileSupportMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileSupportMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}