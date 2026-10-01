namespace AtlasOps.Features.Identity.IdentityLifecycleGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityLifecycleGovernanceView : UserControl
{
    public IdentityLifecycleGovernanceView()
    {
        this.DataContext = new IdentityLifecycleGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityLifecycleGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}