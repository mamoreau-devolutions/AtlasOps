namespace AtlasOps.Features.Security.SecurityBaselineGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBaselineGovernanceView : UserControl
{
    public SecurityBaselineGovernanceView()
    {
        this.DataContext = new SecurityBaselineGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBaselineGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}