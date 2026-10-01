namespace AtlasOps.Features.Api.ApiEndpointMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiEndpointMonitoringView : UserControl
{
    public ApiEndpointMonitoringView()
    {
        this.DataContext = new ApiEndpointMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiEndpointMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}