namespace AtlasOps.Features.Kubernetes.KubernetesVolumeGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesVolumeGovernanceView : UserControl
{
    public KubernetesVolumeGovernanceView()
    {
        this.DataContext = new KubernetesVolumeGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesVolumeGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}