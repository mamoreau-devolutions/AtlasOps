namespace AtlasOps.Features.Security.SecurityFindingProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityFindingProvisioningView : UserControl
{
    public SecurityFindingProvisioningView()
    {
        this.DataContext = new SecurityFindingProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityFindingProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}