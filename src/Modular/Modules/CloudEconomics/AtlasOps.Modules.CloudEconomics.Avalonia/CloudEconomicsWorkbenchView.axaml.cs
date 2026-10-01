namespace AtlasOps.Modules.CloudEconomics.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class CloudEconomicsWorkbenchView : UserControl
{
    public CloudEconomicsWorkbenchView() { this.InitializeComponent(); this.DataContext = new CloudEconomicsWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}