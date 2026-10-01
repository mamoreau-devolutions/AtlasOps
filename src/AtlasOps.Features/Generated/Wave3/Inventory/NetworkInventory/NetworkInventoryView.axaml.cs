namespace AtlasOps.Features.Inventory.NetworkInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkInventoryView : UserControl
{
    public NetworkInventoryView()
    {
        this.DataContext = new NetworkInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}