namespace AtlasOps.Features.Observability.MetricAlertRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricAlertRecoveryView : UserControl
{
    public MetricAlertRecoveryView()
    {
        this.DataContext = new MetricAlertRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricAlertRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}