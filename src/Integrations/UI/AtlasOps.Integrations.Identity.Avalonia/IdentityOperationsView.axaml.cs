namespace AtlasOps.Integrations.Identity.Avalonia;

using global::Avalonia.Controls;

public sealed partial class IdentityOperationsView : UserControl
{
    public IdentityOperationsView()
    {
        this.InitializeComponent();
        this.DataContext = new IdentityOperationsViewModel();
    }
}
