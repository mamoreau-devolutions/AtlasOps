namespace AtlasOps.Features.ServiceManagement.ServiceOwnerMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceOwnerMonitoringView : UserControl
{
    public ServiceOwnerMonitoringView()
    {
        this.DataContext = new ServiceOwnerMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceOwnerMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}