namespace AtlasOps.Features.Desktop.DesktopProfileProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopProfileProvisioningView : UserControl
{
    public DesktopProfileProvisioningView()
    {
        this.DataContext = new DesktopProfileProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopProfileProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}