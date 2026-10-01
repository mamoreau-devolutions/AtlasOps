namespace AtlasOps.Features.Kubernetes.KubernetesClusterProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesClusterProvisioningView : UserControl
{
    public KubernetesClusterProvisioningView()
    {
        this.DataContext = new KubernetesClusterProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesClusterProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}