namespace AtlasOps.Features.Desktop.DesktopImageGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopImageGovernanceView : UserControl
{
    public DesktopImageGovernanceView()
    {
        this.DataContext = new DesktopImageGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopImageGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}