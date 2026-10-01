namespace AtlasOps.Features.Desktop.DesktopApplicationGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopApplicationGovernanceView : UserControl
{
    public DesktopApplicationGovernanceView()
    {
        this.DataContext = new DesktopApplicationGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopApplicationGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}