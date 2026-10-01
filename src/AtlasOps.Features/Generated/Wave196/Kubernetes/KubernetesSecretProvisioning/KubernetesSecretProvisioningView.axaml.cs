namespace AtlasOps.Features.Kubernetes.KubernetesSecretProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesSecretProvisioningView : UserControl
{
    public KubernetesSecretProvisioningView()
    {
        this.DataContext = new KubernetesSecretProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesSecretProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}