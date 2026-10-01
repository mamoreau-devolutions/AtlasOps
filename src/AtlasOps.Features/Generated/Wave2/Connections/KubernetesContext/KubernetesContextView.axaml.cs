namespace AtlasOps.Features.Connections.KubernetesContext;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesContextView : UserControl
{
    public KubernetesContextView()
    {
        this.DataContext = new KubernetesContextViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesContextViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}