namespace AtlasOps.Features.Analytics.NumberWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NumberWidgetView : UserControl
{
    public NumberWidgetView()
    {
        this.DataContext = new NumberWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NumberWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}