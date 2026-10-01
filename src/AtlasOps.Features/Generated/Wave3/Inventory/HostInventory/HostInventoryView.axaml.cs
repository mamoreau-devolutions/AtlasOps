namespace AtlasOps.Features.Inventory.HostInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class HostInventoryView : UserControl
{
    public HostInventoryView()
    {
        this.DataContext = new HostInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is HostInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}