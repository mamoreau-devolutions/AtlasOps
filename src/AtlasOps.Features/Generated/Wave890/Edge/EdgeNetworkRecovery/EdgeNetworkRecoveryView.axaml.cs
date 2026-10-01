namespace AtlasOps.Features.Edge.EdgeNetworkRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeNetworkRecoveryView : UserControl
{
    public EdgeNetworkRecoveryView()
    {
        this.DataContext = new EdgeNetworkRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeNetworkRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}