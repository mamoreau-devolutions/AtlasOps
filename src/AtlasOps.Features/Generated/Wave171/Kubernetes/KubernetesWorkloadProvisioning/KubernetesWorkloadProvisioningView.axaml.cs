namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesWorkloadProvisioningView : UserControl
{
    public KubernetesWorkloadProvisioningView()
    {
        this.DataContext = new KubernetesWorkloadProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesWorkloadProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}