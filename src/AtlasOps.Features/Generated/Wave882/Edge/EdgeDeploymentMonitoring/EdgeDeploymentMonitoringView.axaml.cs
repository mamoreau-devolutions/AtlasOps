namespace AtlasOps.Features.Edge.EdgeDeploymentMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeploymentMonitoringView : UserControl
{
    public EdgeDeploymentMonitoringView()
    {
        this.DataContext = new EdgeDeploymentMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeploymentMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}