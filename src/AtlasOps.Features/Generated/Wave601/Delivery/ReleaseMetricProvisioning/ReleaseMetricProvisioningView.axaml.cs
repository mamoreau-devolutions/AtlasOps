namespace AtlasOps.Features.Delivery.ReleaseMetricProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseMetricProvisioningView : UserControl
{
    public ReleaseMetricProvisioningView()
    {
        this.DataContext = new ReleaseMetricProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseMetricProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}