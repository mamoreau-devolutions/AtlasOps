namespace AtlasOps.Integrations.Communications.Avalonia;

using global::Avalonia.Controls;

public sealed partial class CommunicationsWorkbenchView : UserControl
{
    public CommunicationsWorkbenchView()
    {
        this.InitializeComponent();
        this.DataContext = new CommunicationsWorkbenchViewModel();
    }
}
