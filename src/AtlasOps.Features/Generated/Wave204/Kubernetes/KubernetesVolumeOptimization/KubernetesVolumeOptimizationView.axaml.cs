namespace AtlasOps.Features.Kubernetes.KubernetesVolumeOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesVolumeOptimizationView : UserControl
{
    public KubernetesVolumeOptimizationView()
    {
        this.DataContext = new KubernetesVolumeOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesVolumeOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}