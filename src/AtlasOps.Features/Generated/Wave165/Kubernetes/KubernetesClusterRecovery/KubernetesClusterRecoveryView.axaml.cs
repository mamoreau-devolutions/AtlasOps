namespace AtlasOps.Features.Kubernetes.KubernetesClusterRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesClusterRecoveryView : UserControl
{
    public KubernetesClusterRecoveryView()
    {
        this.DataContext = new KubernetesClusterRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesClusterRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}