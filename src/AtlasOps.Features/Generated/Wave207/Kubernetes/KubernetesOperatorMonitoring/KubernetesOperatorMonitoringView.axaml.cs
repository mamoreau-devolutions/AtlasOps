namespace AtlasOps.Features.Kubernetes.KubernetesOperatorMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesOperatorMonitoringView : UserControl
{
    public KubernetesOperatorMonitoringView()
    {
        this.DataContext = new KubernetesOperatorMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesOperatorMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}