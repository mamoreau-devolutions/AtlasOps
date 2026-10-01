namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryObjectiveGovernanceView : UserControl
{
    public RecoveryObjectiveGovernanceView()
    {
        this.DataContext = new RecoveryObjectiveGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryObjectiveGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}