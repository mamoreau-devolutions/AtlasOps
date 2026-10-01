namespace AtlasOps.Features.Desktop.DesktopSessionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopSessionGovernanceView : UserControl
{
    public DesktopSessionGovernanceView()
    {
        this.DataContext = new DesktopSessionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopSessionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}