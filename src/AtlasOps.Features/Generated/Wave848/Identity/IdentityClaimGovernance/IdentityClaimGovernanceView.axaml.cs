namespace AtlasOps.Features.Identity.IdentityClaimGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityClaimGovernanceView : UserControl
{
    public IdentityClaimGovernanceView()
    {
        this.DataContext = new IdentityClaimGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityClaimGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}