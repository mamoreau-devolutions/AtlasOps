namespace AtlasOps.Features.Analytics.TopologyWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TopologyWidgetView : UserControl
{
    public TopologyWidgetView()
    {
        this.DataContext = new TopologyWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TopologyWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}