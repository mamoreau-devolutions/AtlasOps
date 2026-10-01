namespace AtlasOps.Features.Kubernetes.KubernetesConfigGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesConfigGovernanceView : UserControl
{
    public KubernetesConfigGovernanceView()
    {
        this.DataContext = new KubernetesConfigGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesConfigGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}