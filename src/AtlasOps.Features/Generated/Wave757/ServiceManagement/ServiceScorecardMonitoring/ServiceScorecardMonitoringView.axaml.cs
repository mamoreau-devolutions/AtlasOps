namespace AtlasOps.Features.ServiceManagement.ServiceScorecardMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceScorecardMonitoringView : UserControl
{
    public ServiceScorecardMonitoringView()
    {
        this.DataContext = new ServiceScorecardMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceScorecardMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}