namespace AtlasOps.Features.Analytics.MetricDefinition;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricDefinitionView : UserControl
{
    public MetricDefinitionView()
    {
        this.DataContext = new MetricDefinitionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricDefinitionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}