namespace AtlasOps.Modules.Incidents.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class IncidentsWorkbenchView : UserControl
{
    public IncidentsWorkbenchView() { this.InitializeComponent(); this.DataContext = new IncidentsWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}