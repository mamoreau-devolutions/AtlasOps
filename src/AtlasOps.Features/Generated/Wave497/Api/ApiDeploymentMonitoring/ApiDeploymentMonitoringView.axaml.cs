namespace AtlasOps.Features.Api.ApiDeploymentMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiDeploymentMonitoringView : UserControl
{
    public ApiDeploymentMonitoringView()
    {
        this.DataContext = new ApiDeploymentMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiDeploymentMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}