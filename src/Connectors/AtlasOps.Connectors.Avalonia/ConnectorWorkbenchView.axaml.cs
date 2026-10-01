namespace AtlasOps.Connectors.Avalonia;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class ConnectorWorkbenchView : UserControl
{
    public ConnectorWorkbenchView()
    {
        this.InitializeComponent();
        this.DataContext = new ConnectorWorkbenchViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
