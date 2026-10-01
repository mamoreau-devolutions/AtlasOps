namespace AtlasOps.Features.Security.SecurityPatchGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityPatchGovernanceView : UserControl
{
    public SecurityPatchGovernanceView()
    {
        this.DataContext = new SecurityPatchGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityPatchGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}