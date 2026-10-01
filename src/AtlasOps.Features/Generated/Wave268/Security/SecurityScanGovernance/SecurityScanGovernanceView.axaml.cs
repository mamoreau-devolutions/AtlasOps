namespace AtlasOps.Features.Security.SecurityScanGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityScanGovernanceView : UserControl
{
    public SecurityScanGovernanceView()
    {
        this.DataContext = new SecurityScanGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityScanGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}