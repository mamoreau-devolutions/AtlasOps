namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryFailoverGovernanceView : UserControl
{
    public RecoveryFailoverGovernanceView()
    {
        this.DataContext = new RecoveryFailoverGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryFailoverGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}