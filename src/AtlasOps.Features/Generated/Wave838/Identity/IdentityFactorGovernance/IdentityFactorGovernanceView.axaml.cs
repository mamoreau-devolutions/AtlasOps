namespace AtlasOps.Features.Identity.IdentityFactorGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityFactorGovernanceView : UserControl
{
    public IdentityFactorGovernanceView()
    {
        this.DataContext = new IdentityFactorGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityFactorGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}