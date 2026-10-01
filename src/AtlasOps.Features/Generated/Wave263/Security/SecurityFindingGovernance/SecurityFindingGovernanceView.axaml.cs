namespace AtlasOps.Features.Security.SecurityFindingGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityFindingGovernanceView : UserControl
{
    public SecurityFindingGovernanceView()
    {
        this.DataContext = new SecurityFindingGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityFindingGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}