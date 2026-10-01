namespace AtlasOps.Features.Desktop.DesktopHealthProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopHealthProvisioningView : UserControl
{
    public DesktopHealthProvisioningView()
    {
        this.DataContext = new DesktopHealthProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopHealthProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}