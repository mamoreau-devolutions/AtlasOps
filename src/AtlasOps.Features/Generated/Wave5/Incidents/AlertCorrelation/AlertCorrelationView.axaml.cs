namespace AtlasOps.Features.Incidents.AlertCorrelation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AlertCorrelationView : UserControl
{
    public AlertCorrelationView()
    {
        this.DataContext = new AlertCorrelationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AlertCorrelationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}