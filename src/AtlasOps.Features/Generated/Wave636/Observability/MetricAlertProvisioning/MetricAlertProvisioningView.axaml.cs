namespace AtlasOps.Features.Observability.MetricAlertProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricAlertProvisioningView : UserControl
{
    public MetricAlertProvisioningView()
    {
        this.DataContext = new MetricAlertProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricAlertProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}