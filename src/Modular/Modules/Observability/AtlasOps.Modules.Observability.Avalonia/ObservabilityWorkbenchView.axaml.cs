namespace AtlasOps.Modules.Observability.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class ObservabilityWorkbenchView : UserControl
{
    public ObservabilityWorkbenchView() { this.InitializeComponent(); this.DataContext = new ObservabilityWorkbenchViewModel(); }
    private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }
}