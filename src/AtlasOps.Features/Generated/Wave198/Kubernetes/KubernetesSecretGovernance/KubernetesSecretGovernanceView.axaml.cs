namespace AtlasOps.Features.Kubernetes.KubernetesSecretGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesSecretGovernanceView : UserControl
{
    public KubernetesSecretGovernanceView()
    {
        this.DataContext = new KubernetesSecretGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesSecretGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}