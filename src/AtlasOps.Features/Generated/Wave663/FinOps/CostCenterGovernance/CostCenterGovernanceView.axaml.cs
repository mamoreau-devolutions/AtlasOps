namespace AtlasOps.Features.FinOps.CostCenterGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostCenterGovernanceView : UserControl
{
    public CostCenterGovernanceView()
    {
        this.DataContext = new CostCenterGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostCenterGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}