namespace AtlasOps.Features.FinOps.FinOpsReportGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FinOpsReportGovernanceView : UserControl
{
    public FinOpsReportGovernanceView()
    {
        this.DataContext = new FinOpsReportGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FinOpsReportGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}