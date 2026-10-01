namespace AtlasOps.Features.Edge.EdgeGatewayMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeGatewayMonitoringView : UserControl
{
    public EdgeGatewayMonitoringView()
    {
        this.DataContext = new EdgeGatewayMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeGatewayMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}