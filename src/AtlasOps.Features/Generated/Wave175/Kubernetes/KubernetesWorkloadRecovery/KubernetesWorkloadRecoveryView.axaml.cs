namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesWorkloadRecoveryView : UserControl
{
    public KubernetesWorkloadRecoveryView()
    {
        this.DataContext = new KubernetesWorkloadRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesWorkloadRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}