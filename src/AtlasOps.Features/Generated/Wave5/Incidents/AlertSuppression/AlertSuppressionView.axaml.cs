namespace AtlasOps.Features.Incidents.AlertSuppression;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AlertSuppressionView : UserControl
{
    public AlertSuppressionView()
    {
        this.DataContext = new AlertSuppressionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AlertSuppressionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}