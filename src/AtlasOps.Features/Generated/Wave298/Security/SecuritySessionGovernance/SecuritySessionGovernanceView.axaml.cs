namespace AtlasOps.Features.Security.SecuritySessionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecuritySessionGovernanceView : UserControl
{
    public SecuritySessionGovernanceView()
    {
        this.DataContext = new SecuritySessionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecuritySessionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}