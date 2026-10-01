namespace AtlasOps.Features.Kubernetes.KubernetesIngressMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesIngressMonitoringView : UserControl
{
    public KubernetesIngressMonitoringView()
    {
        this.DataContext = new KubernetesIngressMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesIngressMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}