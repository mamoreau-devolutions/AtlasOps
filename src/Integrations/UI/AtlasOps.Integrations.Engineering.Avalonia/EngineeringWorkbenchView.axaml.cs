namespace AtlasOps.Integrations.Engineering.Avalonia;

using global::Avalonia.Controls;

public sealed partial class EngineeringWorkbenchView : UserControl
{
    public EngineeringWorkbenchView()
    {
        this.InitializeComponent();
        this.DataContext = new EngineeringWorkbenchViewModel();
    }
}
