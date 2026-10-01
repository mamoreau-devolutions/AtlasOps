namespace AtlasOps.Features.Inventory.TopologyProjection;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TopologyProjectionView : UserControl
{
    public TopologyProjectionView()
    {
        this.DataContext = new TopologyProjectionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TopologyProjectionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}