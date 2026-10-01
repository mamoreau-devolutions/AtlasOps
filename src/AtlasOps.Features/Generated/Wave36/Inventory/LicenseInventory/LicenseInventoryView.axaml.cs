namespace AtlasOps.Features.Inventory.LicenseInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LicenseInventoryView : UserControl
{
    public LicenseInventoryView()
    {
        this.DataContext = new LicenseInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LicenseInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}