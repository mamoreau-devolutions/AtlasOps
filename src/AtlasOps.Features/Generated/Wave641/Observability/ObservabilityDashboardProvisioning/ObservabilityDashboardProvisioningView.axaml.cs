namespace AtlasOps.Features.Observability.ObservabilityDashboardProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityDashboardProvisioningView : UserControl
{
    public ObservabilityDashboardProvisioningView()
    {
        this.DataContext = new ObservabilityDashboardProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityDashboardProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}