namespace AtlasOps.Features.Delivery.ReleaseApprovalProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseApprovalProvisioningView : UserControl
{
    public ReleaseApprovalProvisioningView()
    {
        this.DataContext = new ReleaseApprovalProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseApprovalProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}