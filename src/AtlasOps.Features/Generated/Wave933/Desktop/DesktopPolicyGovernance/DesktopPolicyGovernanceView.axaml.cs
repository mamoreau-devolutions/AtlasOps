namespace AtlasOps.Features.Desktop.DesktopPolicyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPolicyGovernanceView : UserControl
{
    public DesktopPolicyGovernanceView()
    {
        this.DataContext = new DesktopPolicyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPolicyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}