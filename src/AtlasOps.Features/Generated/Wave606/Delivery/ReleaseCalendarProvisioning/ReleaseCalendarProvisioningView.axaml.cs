namespace AtlasOps.Features.Delivery.ReleaseCalendarProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseCalendarProvisioningView : UserControl
{
    public ReleaseCalendarProvisioningView()
    {
        this.DataContext = new ReleaseCalendarProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseCalendarProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}