namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryFailoverProvisioningView : UserControl
{
    public RecoveryFailoverProvisioningView()
    {
        this.DataContext = new RecoveryFailoverProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryFailoverProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}