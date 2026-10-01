namespace AtlasOps.Features.Identity.IdentityApplicationGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityApplicationGovernanceView : UserControl
{
    public IdentityApplicationGovernanceView()
    {
        this.DataContext = new IdentityApplicationGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityApplicationGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}