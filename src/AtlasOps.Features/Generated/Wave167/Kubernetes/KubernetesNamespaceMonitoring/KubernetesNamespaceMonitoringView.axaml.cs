namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesNamespaceMonitoringView : UserControl
{
    public KubernetesNamespaceMonitoringView()
    {
        this.DataContext = new KubernetesNamespaceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesNamespaceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}