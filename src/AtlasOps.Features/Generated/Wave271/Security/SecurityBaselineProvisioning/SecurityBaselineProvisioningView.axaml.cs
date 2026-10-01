namespace AtlasOps.Features.Security.SecurityBaselineProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBaselineProvisioningView : UserControl
{
    public SecurityBaselineProvisioningView()
    {
        this.DataContext = new SecurityBaselineProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBaselineProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}