namespace AtlasOps.Integrations.Infrastructure.Avalonia;

using global::Avalonia.Controls;

public sealed partial class InfrastructureWorkbenchView : UserControl
{
    public InfrastructureWorkbenchView()
    {
        this.InitializeComponent();
        this.DataContext = new InfrastructureWorkbenchViewModel();
    }
}
