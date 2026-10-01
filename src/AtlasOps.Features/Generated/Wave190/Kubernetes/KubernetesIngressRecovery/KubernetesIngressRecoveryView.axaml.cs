namespace AtlasOps.Features.Kubernetes.KubernetesIngressRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesIngressRecoveryView : UserControl
{
    public KubernetesIngressRecoveryView()
    {
        this.DataContext = new KubernetesIngressRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesIngressRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}