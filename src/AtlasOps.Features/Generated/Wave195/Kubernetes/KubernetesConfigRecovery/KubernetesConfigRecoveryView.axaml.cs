namespace AtlasOps.Features.Kubernetes.KubernetesConfigRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesConfigRecoveryView : UserControl
{
    public KubernetesConfigRecoveryView()
    {
        this.DataContext = new KubernetesConfigRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesConfigRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}