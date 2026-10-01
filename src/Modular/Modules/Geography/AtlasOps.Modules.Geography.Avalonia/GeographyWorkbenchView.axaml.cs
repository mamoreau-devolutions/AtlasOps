namespace AtlasOps.Modules.Geography.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class GeographyWorkbenchView : UserControl
{
    public GeographyWorkbenchView() { this.InitializeComponent(); this.DataContext = new GeographyWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}