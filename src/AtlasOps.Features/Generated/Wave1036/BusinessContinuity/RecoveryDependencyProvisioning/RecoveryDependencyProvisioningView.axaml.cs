namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryDependencyProvisioningView : UserControl
{
    public RecoveryDependencyProvisioningView()
    {
        this.DataContext = new RecoveryDependencyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryDependencyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}