namespace AtlasOps.Features.Kubernetes.KubernetesServiceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesServiceMonitoringView : UserControl
{
    public KubernetesServiceMonitoringView()
    {
        this.DataContext = new KubernetesServiceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesServiceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}