namespace AtlasOps.Features.Delivery.ReleaseRollbackProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseRollbackProvisioningView : UserControl
{
    public ReleaseRollbackProvisioningView()
    {
        this.DataContext = new ReleaseRollbackProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseRollbackProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}