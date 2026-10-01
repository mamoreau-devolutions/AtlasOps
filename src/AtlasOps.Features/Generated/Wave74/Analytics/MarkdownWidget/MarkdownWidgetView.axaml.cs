namespace AtlasOps.Features.Analytics.MarkdownWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MarkdownWidgetView : UserControl
{
    public MarkdownWidgetView()
    {
        this.DataContext = new MarkdownWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MarkdownWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}