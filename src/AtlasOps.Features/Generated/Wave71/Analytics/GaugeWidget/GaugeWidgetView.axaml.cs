namespace AtlasOps.Features.Analytics.GaugeWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class GaugeWidgetView : UserControl
{
    public GaugeWidgetView()
    {
        this.DataContext = new GaugeWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is GaugeWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}