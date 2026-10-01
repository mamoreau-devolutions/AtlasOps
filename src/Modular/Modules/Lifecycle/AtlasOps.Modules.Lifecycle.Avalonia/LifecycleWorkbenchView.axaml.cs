namespace AtlasOps.Modules.Lifecycle.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class LifecycleWorkbenchView : UserControl
{
    public LifecycleWorkbenchView() { this.InitializeComponent(); this.DataContext = new LifecycleWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}