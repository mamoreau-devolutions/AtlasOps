namespace AtlasOps.Features.Desktop.DesktopProfileGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopProfileGovernanceView : UserControl
{
    public DesktopProfileGovernanceView()
    {
        this.DataContext = new DesktopProfileGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopProfileGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}