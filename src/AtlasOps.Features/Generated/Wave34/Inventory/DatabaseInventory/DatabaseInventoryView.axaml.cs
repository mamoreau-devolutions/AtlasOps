namespace AtlasOps.Features.Inventory.DatabaseInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseInventoryView : UserControl
{
    public DatabaseInventoryView()
    {
        this.DataContext = new DatabaseInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}