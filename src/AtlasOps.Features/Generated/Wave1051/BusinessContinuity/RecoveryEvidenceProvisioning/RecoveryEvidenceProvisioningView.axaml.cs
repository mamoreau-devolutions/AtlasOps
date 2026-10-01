namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryEvidenceProvisioningView : UserControl
{
    public RecoveryEvidenceProvisioningView()
    {
        this.DataContext = new RecoveryEvidenceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryEvidenceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}