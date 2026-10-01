namespace AtlasOps.Features.Analytics.MetricSample;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricSampleView : UserControl
{
    public MetricSampleView()
    {
        this.DataContext = new MetricSampleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricSampleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}