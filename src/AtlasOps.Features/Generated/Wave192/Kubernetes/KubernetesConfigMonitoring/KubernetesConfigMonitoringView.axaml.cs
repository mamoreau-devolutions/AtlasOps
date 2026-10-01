namespace AtlasOps.Features.Kubernetes.KubernetesConfigMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesConfigMonitoringView : UserControl
{
    public KubernetesConfigMonitoringView()
    {
        this.DataContext = new KubernetesConfigMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesConfigMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}