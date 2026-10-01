namespace AtlasOps.Features.Desktop.DesktopImageProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopImageProvisioningView : UserControl
{
    public DesktopImageProvisioningView()
    {
        this.DataContext = new DesktopImageProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopImageProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}