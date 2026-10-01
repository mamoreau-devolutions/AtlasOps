namespace AtlasOps.Features.Security.SecurityIdentityGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityIdentityGovernanceView : UserControl
{
    public SecurityIdentityGovernanceView()
    {
        this.DataContext = new SecurityIdentityGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityIdentityGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}