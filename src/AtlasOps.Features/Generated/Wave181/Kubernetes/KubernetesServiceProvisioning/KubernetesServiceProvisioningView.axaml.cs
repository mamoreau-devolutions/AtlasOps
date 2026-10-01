namespace AtlasOps.Features.Kubernetes.KubernetesServiceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesServiceProvisioningView : UserControl
{
    public KubernetesServiceProvisioningView()
    {
        this.DataContext = new KubernetesServiceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesServiceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}