namespace AtlasOps.Modules.Deployments.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class DeploymentsWorkbenchView : UserControl
{
    public DeploymentsWorkbenchView() { this.InitializeComponent(); this.DataContext = new DeploymentsWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}