namespace AtlasOps.Features.Analytics.HealthWidget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class HealthWidgetView : UserControl
{
    public HealthWidgetView()
    {
        this.DataContext = new HealthWidgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is HealthWidgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}