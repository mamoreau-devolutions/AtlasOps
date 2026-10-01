namespace AtlasOps.Features.Identity.IdentitySessionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentitySessionGovernanceView : UserControl
{
    public IdentitySessionGovernanceView()
    {
        this.DataContext = new IdentitySessionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentitySessionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}