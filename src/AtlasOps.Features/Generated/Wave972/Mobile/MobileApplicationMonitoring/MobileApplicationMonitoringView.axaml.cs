namespace AtlasOps.Features.Mobile.MobileApplicationMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileApplicationMonitoringView : UserControl
{
    public MobileApplicationMonitoringView()
    {
        this.DataContext = new MobileApplicationMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileApplicationMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}