namespace AtlasOps.Features.Identity.IdentityAuditGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityAuditGovernanceView : UserControl
{
    public IdentityAuditGovernanceView()
    {
        this.DataContext = new IdentityAuditGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityAuditGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}