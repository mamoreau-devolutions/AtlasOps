namespace AtlasOps.Modules.Governance.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class GovernanceWorkbenchView : UserControl
{
    public GovernanceWorkbenchView() { this.InitializeComponent(); this.DataContext = new GovernanceWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}