namespace AtlasOps.Features.Kubernetes.KubernetesIngressGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesIngressGovernanceView : UserControl
{
    public KubernetesIngressGovernanceView()
    {
        this.DataContext = new KubernetesIngressGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesIngressGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}