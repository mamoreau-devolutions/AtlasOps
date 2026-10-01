namespace AtlasOps.Features.Observability.ObservabilityDashboardRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityDashboardRecoveryView : UserControl
{
    public ObservabilityDashboardRecoveryView()
    {
        this.DataContext = new ObservabilityDashboardRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityDashboardRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}