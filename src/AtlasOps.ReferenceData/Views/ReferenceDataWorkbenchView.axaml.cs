namespace AtlasOps.ReferenceData.Views;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

public sealed partial class ReferenceDataWorkbenchView : UserControl
{
    public ReferenceDataWorkbenchView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}