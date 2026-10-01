namespace AtlasOps.Features.Kubernetes.KubernetesServiceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesServiceRecoveryView : UserControl
{
    public KubernetesServiceRecoveryView()
    {
        this.DataContext = new KubernetesServiceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesServiceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}