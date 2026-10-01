namespace AtlasOps.Features.Desktop.DesktopUpdateGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopUpdateGovernanceView : UserControl
{
    public DesktopUpdateGovernanceView()
    {
        this.DataContext = new DesktopUpdateGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopUpdateGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}