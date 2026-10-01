namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ContinuityPlanRecoveryView : UserControl
{
    public ContinuityPlanRecoveryView()
    {
        this.DataContext = new ContinuityPlanRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ContinuityPlanRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}