namespace AtlasOps.Features.Kubernetes.KubernetesSecretMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesSecretMonitoringView : UserControl
{
    public KubernetesSecretMonitoringView()
    {
        this.DataContext = new KubernetesSecretMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesSecretMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}