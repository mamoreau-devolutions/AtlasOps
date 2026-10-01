namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ContinuityPlanGovernanceView : UserControl
{
    public ContinuityPlanGovernanceView()
    {
        this.DataContext = new ContinuityPlanGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ContinuityPlanGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}