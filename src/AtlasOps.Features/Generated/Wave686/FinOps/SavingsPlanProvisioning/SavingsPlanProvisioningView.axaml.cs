namespace AtlasOps.Features.FinOps.SavingsPlanProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SavingsPlanProvisioningView : UserControl
{
    public SavingsPlanProvisioningView()
    {
        this.DataContext = new SavingsPlanProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SavingsPlanProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}