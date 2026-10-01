namespace AtlasOps.Features.Kubernetes.KubernetesOperatorRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesOperatorRecoveryView : UserControl
{
    public KubernetesOperatorRecoveryView()
    {
        this.DataContext = new KubernetesOperatorRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesOperatorRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}