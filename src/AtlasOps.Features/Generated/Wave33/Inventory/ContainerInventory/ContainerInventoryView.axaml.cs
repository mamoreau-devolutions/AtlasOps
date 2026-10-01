namespace AtlasOps.Features.Inventory.ContainerInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ContainerInventoryView : UserControl
{
    public ContainerInventoryView()
    {
        this.DataContext = new ContainerInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ContainerInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}