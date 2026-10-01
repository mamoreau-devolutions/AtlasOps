namespace AtlasOps.Features.Identity.IdentityRoleGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityRoleGovernanceView : UserControl
{
    public IdentityRoleGovernanceView()
    {
        this.DataContext = new IdentityRoleGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityRoleGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}