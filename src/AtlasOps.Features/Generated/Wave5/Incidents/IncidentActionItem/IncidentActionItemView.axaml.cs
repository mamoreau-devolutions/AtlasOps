namespace AtlasOps.Features.Incidents.IncidentActionItem;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IncidentActionItemView : UserControl
{
    public IncidentActionItemView()
    {
        this.DataContext = new IncidentActionItemViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IncidentActionItemViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}