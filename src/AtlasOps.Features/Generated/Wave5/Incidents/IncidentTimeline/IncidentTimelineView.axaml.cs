namespace AtlasOps.Features.Incidents.IncidentTimeline;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IncidentTimelineView : UserControl
{
    public IncidentTimelineView()
    {
        this.DataContext = new IncidentTimelineViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IncidentTimelineViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}