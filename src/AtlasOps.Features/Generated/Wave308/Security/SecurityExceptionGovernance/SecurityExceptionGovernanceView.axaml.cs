namespace AtlasOps.Features.Security.SecurityExceptionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityExceptionGovernanceView : UserControl
{
    public SecurityExceptionGovernanceView()
    {
        this.DataContext = new SecurityExceptionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityExceptionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}