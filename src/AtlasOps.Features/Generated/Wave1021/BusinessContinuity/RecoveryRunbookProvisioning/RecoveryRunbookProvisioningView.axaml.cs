namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryRunbookProvisioningView : UserControl
{
    public RecoveryRunbookProvisioningView()
    {
        this.DataContext = new RecoveryRunbookProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryRunbookProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}