namespace AtlasOps.Features.Inventory.ServiceInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceInventoryView : UserControl
{
    public ServiceInventoryView()
    {
        this.DataContext = new ServiceInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}