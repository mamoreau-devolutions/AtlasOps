namespace AtlasOps.Features.Security.SecurityExceptionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityExceptionProvisioningView : UserControl
{
    public SecurityExceptionProvisioningView()
    {
        this.DataContext = new SecurityExceptionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityExceptionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}