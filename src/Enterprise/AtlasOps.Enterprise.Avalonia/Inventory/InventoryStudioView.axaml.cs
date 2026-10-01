namespace AtlasOps.Enterprise.Avalonia.Inventory;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class InventoryStudioView : UserControl
{
    public InventoryStudioView()
    {
        this.InitializeComponent();
        this.DataContext = new InventoryStudioViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
