namespace AtlasOps.Features.Desktop.DesktopApplicationProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopApplicationProvisioningView : UserControl
{
    public DesktopApplicationProvisioningView()
    {
        this.DataContext = new DesktopApplicationProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopApplicationProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}