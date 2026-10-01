namespace AtlasOps.Features.Identity.IdentityProviderGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityProviderGovernanceView : UserControl
{
    public IdentityProviderGovernanceView()
    {
        this.DataContext = new IdentityProviderGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityProviderGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}