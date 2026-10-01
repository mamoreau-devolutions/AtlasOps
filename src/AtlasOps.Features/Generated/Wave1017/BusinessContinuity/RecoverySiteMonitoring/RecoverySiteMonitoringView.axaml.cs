namespace AtlasOps.Features.BusinessContinuity.RecoverySiteMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoverySiteMonitoringView : UserControl
{
    public RecoverySiteMonitoringView()
    {
        this.DataContext = new RecoverySiteMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoverySiteMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}