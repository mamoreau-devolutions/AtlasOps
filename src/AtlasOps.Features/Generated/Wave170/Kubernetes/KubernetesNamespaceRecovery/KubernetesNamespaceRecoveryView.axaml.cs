namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesNamespaceRecoveryView : UserControl
{
    public KubernetesNamespaceRecoveryView()
    {
        this.DataContext = new KubernetesNamespaceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesNamespaceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}