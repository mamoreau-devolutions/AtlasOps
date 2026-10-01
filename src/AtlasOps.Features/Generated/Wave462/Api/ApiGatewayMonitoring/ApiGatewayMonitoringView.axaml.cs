namespace AtlasOps.Features.Api.ApiGatewayMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiGatewayMonitoringView : UserControl
{
    public ApiGatewayMonitoringView()
    {
        this.DataContext = new ApiGatewayMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiGatewayMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}