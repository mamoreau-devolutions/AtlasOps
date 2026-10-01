namespace AtlasOps.Features.Desktop.DesktopPolicyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPolicyProvisioningView : UserControl
{
    public DesktopPolicyProvisioningView()
    {
        this.DataContext = new DesktopPolicyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPolicyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}