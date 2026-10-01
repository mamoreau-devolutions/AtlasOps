namespace AtlasOps.Features.Mobile.MobilePolicyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobilePolicyMonitoringView : UserControl
{
    public MobilePolicyMonitoringView()
    {
        this.DataContext = new MobilePolicyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobilePolicyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}