namespace AtlasOps.Features.Kubernetes.KubernetesClusterMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesClusterMonitoringView : UserControl
{
    public KubernetesClusterMonitoringView()
    {
        this.DataContext = new KubernetesClusterMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesClusterMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}