namespace AtlasOps.Features.Desktop.DesktopHealthGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopHealthGovernanceView : UserControl
{
    public DesktopHealthGovernanceView()
    {
        this.DataContext = new DesktopHealthGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopHealthGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}