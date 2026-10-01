namespace AtlasOps.Modules.NetworkIntelligence.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class NetworkIntelligenceWorkbenchView : UserControl
{
    public NetworkIntelligenceWorkbenchView() { this.InitializeComponent(); this.DataContext = new NetworkIntelligenceWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}