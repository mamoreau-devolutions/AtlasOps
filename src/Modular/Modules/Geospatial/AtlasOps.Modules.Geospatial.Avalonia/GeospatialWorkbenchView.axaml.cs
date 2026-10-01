namespace AtlasOps.Modules.Geospatial.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class GeospatialWorkbenchView : UserControl
{
    public GeospatialWorkbenchView() { this.InitializeComponent(); this.DataContext = new GeospatialWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}