namespace AtlasOps.Features.Desktop.DesktopPoolGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPoolGovernanceView : UserControl
{
    public DesktopPoolGovernanceView()
    {
        this.DataContext = new DesktopPoolGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPoolGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}