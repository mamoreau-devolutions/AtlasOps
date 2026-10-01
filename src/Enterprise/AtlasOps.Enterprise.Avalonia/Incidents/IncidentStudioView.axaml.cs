namespace AtlasOps.Enterprise.Avalonia.Incidents;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class IncidentStudioView : UserControl
{
    public IncidentStudioView()
    {
        this.InitializeComponent();
        this.DataContext = new IncidentStudioViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
