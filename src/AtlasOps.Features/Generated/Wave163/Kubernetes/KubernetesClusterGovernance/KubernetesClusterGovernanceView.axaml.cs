namespace AtlasOps.Features.Kubernetes.KubernetesClusterGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesClusterGovernanceView : UserControl
{
    public KubernetesClusterGovernanceView()
    {
        this.DataContext = new KubernetesClusterGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesClusterGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}