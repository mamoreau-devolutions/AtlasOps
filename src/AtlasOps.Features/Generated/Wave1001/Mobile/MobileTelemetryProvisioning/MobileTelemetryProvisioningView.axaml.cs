namespace AtlasOps.Features.Mobile.MobileTelemetryProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileTelemetryProvisioningView : UserControl
{
    public MobileTelemetryProvisioningView()
    {
        this.DataContext = new MobileTelemetryProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileTelemetryProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}