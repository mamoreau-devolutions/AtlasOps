namespace AtlasOps.Operations.Avalonia;

using global::Avalonia.Controls;

public sealed partial class OperationsCommandCenterView : UserControl
{
    public OperationsCommandCenterView()
    {
        this.InitializeComponent();
        this.DataContext = new OperationsCommandCenterViewModel();
    }
}
