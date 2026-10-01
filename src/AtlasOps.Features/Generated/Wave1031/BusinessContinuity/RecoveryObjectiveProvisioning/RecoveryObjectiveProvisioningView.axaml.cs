namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryObjectiveProvisioningView : UserControl
{
    public RecoveryObjectiveProvisioningView()
    {
        this.DataContext = new RecoveryObjectiveProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryObjectiveProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}