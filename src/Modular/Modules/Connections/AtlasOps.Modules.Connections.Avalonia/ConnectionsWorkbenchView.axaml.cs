namespace AtlasOps.Modules.Connections.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class ConnectionsWorkbenchView : UserControl
{
    public ConnectionsWorkbenchView() { this.InitializeComponent(); this.DataContext = new ConnectionsWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}