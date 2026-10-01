namespace AtlasOps.Features.Observability.ObservabilityDashboardGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityDashboardGovernanceView : UserControl
{
    public ObservabilityDashboardGovernanceView()
    {
        this.DataContext = new ObservabilityDashboardGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityDashboardGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}