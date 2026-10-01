namespace AtlasOps.Features.Kubernetes.KubernetesVolumeProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesVolumeProvisioningView : UserControl
{
    public KubernetesVolumeProvisioningView()
    {
        this.DataContext = new KubernetesVolumeProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesVolumeProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}