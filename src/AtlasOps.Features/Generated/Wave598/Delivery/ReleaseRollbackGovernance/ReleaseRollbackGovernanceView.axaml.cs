namespace AtlasOps.Features.Delivery.ReleaseRollbackGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseRollbackGovernanceView : UserControl
{
    public ReleaseRollbackGovernanceView()
    {
        this.DataContext = new ReleaseRollbackGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseRollbackGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}