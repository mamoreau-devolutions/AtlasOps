namespace AtlasOps.Modules.Compliance.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class ComplianceWorkbenchView : UserControl
{
    public ComplianceWorkbenchView() { this.InitializeComponent(); this.DataContext = new ComplianceWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}