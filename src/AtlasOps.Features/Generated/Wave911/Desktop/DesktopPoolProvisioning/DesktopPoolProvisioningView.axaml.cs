namespace AtlasOps.Features.Desktop.DesktopPoolProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPoolProvisioningView : UserControl
{
    public DesktopPoolProvisioningView()
    {
        this.DataContext = new DesktopPoolProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPoolProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}