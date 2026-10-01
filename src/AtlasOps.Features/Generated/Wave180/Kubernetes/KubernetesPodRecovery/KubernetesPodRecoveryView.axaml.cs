namespace AtlasOps.Features.Kubernetes.KubernetesPodRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesPodRecoveryView : UserControl
{
    public KubernetesPodRecoveryView()
    {
        this.DataContext = new KubernetesPodRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesPodRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}