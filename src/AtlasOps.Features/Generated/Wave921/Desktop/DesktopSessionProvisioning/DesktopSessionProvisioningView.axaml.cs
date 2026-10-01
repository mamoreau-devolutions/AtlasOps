namespace AtlasOps.Features.Desktop.DesktopSessionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopSessionProvisioningView : UserControl
{
    public DesktopSessionProvisioningView()
    {
        this.DataContext = new DesktopSessionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopSessionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}