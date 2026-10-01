namespace AtlasOps.Modules.Inventory.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class InventoryWorkbenchView : UserControl
{
    public InventoryWorkbenchView() { this.InitializeComponent(); this.DataContext = new InventoryWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}