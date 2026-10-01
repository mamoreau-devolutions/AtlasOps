namespace AtlasOps.Features.FinOps.CostAllocationGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAllocationGovernanceView : UserControl
{
    public CostAllocationGovernanceView()
    {
        this.DataContext = new CostAllocationGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAllocationGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}