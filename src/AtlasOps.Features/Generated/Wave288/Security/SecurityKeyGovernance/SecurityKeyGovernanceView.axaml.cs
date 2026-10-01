namespace AtlasOps.Features.Security.SecurityKeyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityKeyGovernanceView : UserControl
{
    public SecurityKeyGovernanceView()
    {
        this.DataContext = new SecurityKeyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityKeyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}