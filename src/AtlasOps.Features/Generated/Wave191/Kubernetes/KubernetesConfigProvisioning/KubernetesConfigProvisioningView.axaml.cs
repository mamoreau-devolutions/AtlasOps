namespace AtlasOps.Features.Kubernetes.KubernetesConfigProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesConfigProvisioningView : UserControl
{
    public KubernetesConfigProvisioningView()
    {
        this.DataContext = new KubernetesConfigProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesConfigProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}