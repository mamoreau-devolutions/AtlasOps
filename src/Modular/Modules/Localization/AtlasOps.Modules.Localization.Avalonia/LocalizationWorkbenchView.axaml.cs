namespace AtlasOps.Modules.Localization.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class LocalizationWorkbenchView : UserControl
{
    public LocalizationWorkbenchView() { this.InitializeComponent(); this.DataContext = new LocalizationWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}