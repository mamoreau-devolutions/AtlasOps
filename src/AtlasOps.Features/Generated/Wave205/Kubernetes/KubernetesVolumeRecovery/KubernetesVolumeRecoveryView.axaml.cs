namespace AtlasOps.Features.Kubernetes.KubernetesVolumeRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesVolumeRecoveryView : UserControl
{
    public KubernetesVolumeRecoveryView()
    {
        this.DataContext = new KubernetesVolumeRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesVolumeRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}