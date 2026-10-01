namespace AtlasOps.Features.Observability.MetricSourceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricSourceRecoveryView : UserControl
{
    public MetricSourceRecoveryView()
    {
        this.DataContext = new MetricSourceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricSourceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}