namespace AtlasOps.Enterprise.Avalonia.Workflow;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class WorkflowStudioView : UserControl
{
    public WorkflowStudioView()
    {
        this.InitializeComponent();
        this.DataContext = new WorkflowStudioViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
