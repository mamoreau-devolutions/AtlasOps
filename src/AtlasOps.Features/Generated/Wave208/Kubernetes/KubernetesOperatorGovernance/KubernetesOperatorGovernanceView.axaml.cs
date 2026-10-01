namespace AtlasOps.Features.Kubernetes.KubernetesOperatorGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesOperatorGovernanceView : UserControl
{
    public KubernetesOperatorGovernanceView()
    {
        this.DataContext = new KubernetesOperatorGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesOperatorGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}