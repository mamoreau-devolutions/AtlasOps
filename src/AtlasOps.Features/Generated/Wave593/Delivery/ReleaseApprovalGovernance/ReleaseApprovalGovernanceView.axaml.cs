namespace AtlasOps.Features.Delivery.ReleaseApprovalGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseApprovalGovernanceView : UserControl
{
    public ReleaseApprovalGovernanceView()
    {
        this.DataContext = new ReleaseApprovalGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseApprovalGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}