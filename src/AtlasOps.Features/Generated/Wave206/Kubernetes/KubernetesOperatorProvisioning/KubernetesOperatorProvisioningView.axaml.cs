namespace AtlasOps.Features.Kubernetes.KubernetesOperatorProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesOperatorProvisioningView : UserControl
{
    public KubernetesOperatorProvisioningView()
    {
        this.DataContext = new KubernetesOperatorProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesOperatorProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}