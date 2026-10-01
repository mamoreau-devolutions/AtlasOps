namespace AtlasOps.Modules.Documents.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class DocumentsWorkbenchView : UserControl
{
    public DocumentsWorkbenchView() { this.InitializeComponent(); this.DataContext = new DocumentsWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}