namespace AtlasOps.Enterprise.Avalonia.Policy;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

public sealed partial class PolicyStudioView : UserControl
{
    public PolicyStudioView()
    {
        this.InitializeComponent();
        this.DataContext = new PolicyStudioViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
