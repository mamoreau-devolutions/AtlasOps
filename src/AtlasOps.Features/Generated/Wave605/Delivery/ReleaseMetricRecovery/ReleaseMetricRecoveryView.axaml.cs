namespace AtlasOps.Features.Delivery.ReleaseMetricRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseMetricRecoveryView : UserControl
{
    public ReleaseMetricRecoveryView()
    {
        this.DataContext = new ReleaseMetricRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseMetricRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}