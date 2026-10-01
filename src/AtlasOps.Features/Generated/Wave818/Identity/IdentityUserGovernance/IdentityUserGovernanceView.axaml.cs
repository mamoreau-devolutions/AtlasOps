namespace AtlasOps.Features.Identity.IdentityUserGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityUserGovernanceView : UserControl
{
    public IdentityUserGovernanceView()
    {
        this.DataContext = new IdentityUserGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityUserGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}