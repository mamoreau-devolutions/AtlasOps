namespace AtlasOps.Features.Edge.EdgeSiteRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeSiteRecoveryView : UserControl
{
    public EdgeSiteRecoveryView()
    {
        this.DataContext = new EdgeSiteRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeSiteRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}