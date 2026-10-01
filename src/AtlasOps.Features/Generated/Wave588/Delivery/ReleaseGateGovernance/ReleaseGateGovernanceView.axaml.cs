namespace AtlasOps.Features.Delivery.ReleaseGateGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseGateGovernanceView : UserControl
{
    public ReleaseGateGovernanceView()
    {
        this.DataContext = new ReleaseGateGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseGateGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}