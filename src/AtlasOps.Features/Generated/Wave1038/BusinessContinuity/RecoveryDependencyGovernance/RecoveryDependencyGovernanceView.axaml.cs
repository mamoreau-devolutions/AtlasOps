namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryDependencyGovernanceView : UserControl
{
    public RecoveryDependencyGovernanceView()
    {
        this.DataContext = new RecoveryDependencyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryDependencyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}