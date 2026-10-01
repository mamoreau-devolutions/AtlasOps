namespace AtlasOps.Features.Desktop.DesktopPeripheralGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPeripheralGovernanceView : UserControl
{
    public DesktopPeripheralGovernanceView()
    {
        this.DataContext = new DesktopPeripheralGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPeripheralGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}