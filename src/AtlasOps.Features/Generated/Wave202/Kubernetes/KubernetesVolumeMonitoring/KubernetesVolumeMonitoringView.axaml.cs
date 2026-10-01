namespace AtlasOps.Features.Kubernetes.KubernetesVolumeMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesVolumeMonitoringView : UserControl
{
    public KubernetesVolumeMonitoringView()
    {
        this.DataContext = new KubernetesVolumeMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesVolumeMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}