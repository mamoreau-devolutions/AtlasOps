namespace AtlasOps.Modules.Workspaces.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class WorkspacesWorkbenchView : UserControl
{
    public WorkspacesWorkbenchView() { this.InitializeComponent(); this.DataContext = new WorkspacesWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}