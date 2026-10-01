namespace AtlasOps.Features.Analytics.TimelineWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TimelineWidgetView : UserControl
{
    public TimelineWidgetView()
    {
        this.DataContext = new TimelineWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TimelineWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}