namespace AtlasOps.Features.Delivery.ReleaseGateProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseGateProvisioningView : UserControl
{
    public ReleaseGateProvisioningView()
    {
        this.DataContext = new ReleaseGateProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseGateProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}