namespace AtlasOps.Features.Identity.IdentityGroupGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityGroupGovernanceView : UserControl
{
    public IdentityGroupGovernanceView()
    {
        this.DataContext = new IdentityGroupGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityGroupGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}