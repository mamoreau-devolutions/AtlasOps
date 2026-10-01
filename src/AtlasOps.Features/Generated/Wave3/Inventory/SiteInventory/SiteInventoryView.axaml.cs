namespace AtlasOps.Features.Inventory.SiteInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SiteInventoryView : UserControl
{
    public SiteInventoryView()
    {
        this.DataContext = new SiteInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SiteInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}