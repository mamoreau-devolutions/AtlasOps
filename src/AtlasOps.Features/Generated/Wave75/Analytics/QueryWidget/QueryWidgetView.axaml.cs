namespace AtlasOps.Features.Analytics.QueryWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class QueryWidgetView : UserControl
{
    public QueryWidgetView()
    {
        this.DataContext = new QueryWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is QueryWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}