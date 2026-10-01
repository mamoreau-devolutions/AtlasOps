namespace AtlasOps.Features.Kubernetes.KubernetesPodProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesPodProvisioningView : UserControl
{
    public KubernetesPodProvisioningView()
    {
        this.DataContext = new KubernetesPodProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesPodProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}