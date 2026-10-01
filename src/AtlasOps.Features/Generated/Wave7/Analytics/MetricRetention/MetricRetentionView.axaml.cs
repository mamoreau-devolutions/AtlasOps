namespace AtlasOps.Features.Analytics.MetricRetention;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricRetentionView : UserControl
{
    public MetricRetentionView()
    {
        this.DataContext = new MetricRetentionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricRetentionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}