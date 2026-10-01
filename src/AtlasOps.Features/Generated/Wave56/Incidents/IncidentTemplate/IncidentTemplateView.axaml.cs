namespace AtlasOps.Features.Incidents.IncidentTemplate;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IncidentTemplateView : UserControl
{
    public IncidentTemplateView()
    {
        this.DataContext = new IncidentTemplateViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IncidentTemplateViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}