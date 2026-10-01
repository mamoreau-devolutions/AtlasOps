namespace AtlasOps.Features.Security.SecurityBoundaryGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBoundaryGovernanceView : UserControl
{
    public SecurityBoundaryGovernanceView()
    {
        this.DataContext = new SecurityBoundaryGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBoundaryGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}