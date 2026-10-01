namespace AtlasOps.Features.ServiceManagement.ServiceDependencyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceDependencyMonitoringView : UserControl
{
    public ServiceDependencyMonitoringView()
    {
        this.DataContext = new ServiceDependencyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceDependencyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}