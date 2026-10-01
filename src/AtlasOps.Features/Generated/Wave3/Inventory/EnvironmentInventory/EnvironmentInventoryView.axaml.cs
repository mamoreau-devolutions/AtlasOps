namespace AtlasOps.Features.Inventory.EnvironmentInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EnvironmentInventoryView : UserControl
{
    public EnvironmentInventoryView()
    {
        this.DataContext = new EnvironmentInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EnvironmentInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}