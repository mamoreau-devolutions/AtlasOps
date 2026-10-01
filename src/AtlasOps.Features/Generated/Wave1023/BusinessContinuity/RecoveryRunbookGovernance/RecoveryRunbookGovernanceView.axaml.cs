namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryRunbookGovernanceView : UserControl
{
    public RecoveryRunbookGovernanceView()
    {
        this.DataContext = new RecoveryRunbookGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryRunbookGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}