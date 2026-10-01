namespace AtlasOps.Features.Analytics.TableWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TableWidgetView : UserControl
{
    public TableWidgetView()
    {
        this.DataContext = new TableWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TableWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}