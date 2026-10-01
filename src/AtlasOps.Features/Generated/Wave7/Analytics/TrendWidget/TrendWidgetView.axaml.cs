namespace AtlasOps.Features.Analytics.TrendWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TrendWidgetView : UserControl
{
    public TrendWidgetView()
    {
        this.DataContext = new TrendWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TrendWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}