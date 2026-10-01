namespace AtlasOps.Features.FinOps.CostAnomalyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAnomalyGovernanceView : UserControl
{
    public CostAnomalyGovernanceView()
    {
        this.DataContext = new CostAnomalyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAnomalyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}