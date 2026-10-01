namespace AtlasOps.Enterprise.Avalonia.Reporting;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class ReportingStudioView : UserControl
{
    public ReportingStudioView()
    {
        this.InitializeComponent();
        this.DataContext = new ReportingStudioViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
