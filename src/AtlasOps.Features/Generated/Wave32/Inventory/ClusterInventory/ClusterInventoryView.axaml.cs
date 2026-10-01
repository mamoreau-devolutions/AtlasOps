namespace AtlasOps.Features.Inventory.ClusterInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ClusterInventoryView : UserControl
{
    public ClusterInventoryView()
    {
        this.DataContext = new ClusterInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ClusterInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}