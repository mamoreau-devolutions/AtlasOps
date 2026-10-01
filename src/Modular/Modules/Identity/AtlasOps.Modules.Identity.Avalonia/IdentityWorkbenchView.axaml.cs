namespace AtlasOps.Modules.Identity.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class IdentityWorkbenchView : UserControl
{
    public IdentityWorkbenchView() { this.InitializeComponent(); this.DataContext = new IdentityWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}