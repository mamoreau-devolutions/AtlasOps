namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesWorkloadMonitoringView : UserControl
{
    public KubernetesWorkloadMonitoringView()
    {
        this.DataContext = new KubernetesWorkloadMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesWorkloadMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}