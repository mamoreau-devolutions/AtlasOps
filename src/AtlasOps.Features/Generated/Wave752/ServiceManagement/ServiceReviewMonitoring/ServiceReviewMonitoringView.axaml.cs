namespace AtlasOps.Features.ServiceManagement.ServiceReviewMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceReviewMonitoringView : UserControl
{
    public ServiceReviewMonitoringView()
    {
        this.DataContext = new ServiceReviewMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceReviewMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}