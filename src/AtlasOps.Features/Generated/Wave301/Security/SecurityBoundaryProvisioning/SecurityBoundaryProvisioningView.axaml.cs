namespace AtlasOps.Features.Security.SecurityBoundaryProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBoundaryProvisioningView : UserControl
{
    public SecurityBoundaryProvisioningView()
    {
        this.DataContext = new SecurityBoundaryProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBoundaryProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}