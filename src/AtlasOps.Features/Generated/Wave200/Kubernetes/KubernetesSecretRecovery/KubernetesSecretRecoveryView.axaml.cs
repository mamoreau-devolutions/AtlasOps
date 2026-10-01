namespace AtlasOps.Features.Kubernetes.KubernetesSecretRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesSecretRecoveryView : UserControl
{
    public KubernetesSecretRecoveryView()
    {
        this.DataContext = new KubernetesSecretRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesSecretRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}