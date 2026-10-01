namespace AtlasOps.Features.Kubernetes.KubernetesIngressProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesIngressProvisioningView : UserControl
{
    public KubernetesIngressProvisioningView()
    {
        this.DataContext = new KubernetesIngressProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesIngressProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}