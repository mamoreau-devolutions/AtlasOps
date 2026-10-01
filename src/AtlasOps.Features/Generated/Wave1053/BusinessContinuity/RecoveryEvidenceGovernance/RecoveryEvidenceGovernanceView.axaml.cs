namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryEvidenceGovernanceView : UserControl
{
    public RecoveryEvidenceGovernanceView()
    {
        this.DataContext = new RecoveryEvidenceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryEvidenceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}