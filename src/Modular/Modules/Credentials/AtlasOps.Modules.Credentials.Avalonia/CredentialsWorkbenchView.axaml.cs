namespace AtlasOps.Modules.Credentials.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class CredentialsWorkbenchView : UserControl
{
    public CredentialsWorkbenchView() { this.InitializeComponent(); this.DataContext = new CredentialsWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}