namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryBackupProvisioningView : UserControl
{
    public RecoveryBackupProvisioningView()
    {
        this.DataContext = new RecoveryBackupProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryBackupProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}