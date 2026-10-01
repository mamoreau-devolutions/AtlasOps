namespace AtlasOps.Features.Incidents.AlertEnrichment;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AlertEnrichmentView : UserControl
{
    public AlertEnrichmentView()
    {
        this.DataContext = new AlertEnrichmentViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AlertEnrichmentViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}