namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryBackupGovernanceView : UserControl
{
    public RecoveryBackupGovernanceView()
    {
        this.DataContext = new RecoveryBackupGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryBackupGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}