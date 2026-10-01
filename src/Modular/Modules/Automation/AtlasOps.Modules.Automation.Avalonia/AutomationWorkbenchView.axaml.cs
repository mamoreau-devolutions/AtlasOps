namespace AtlasOps.Modules.Automation.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class AutomationWorkbenchView : UserControl
{
    public AutomationWorkbenchView() { this.InitializeComponent(); this.DataContext = new AutomationWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}