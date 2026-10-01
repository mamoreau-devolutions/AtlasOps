namespace AtlasOps.Features.Desktop.DesktopLicenseGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopLicenseGovernanceView : UserControl
{
    public DesktopLicenseGovernanceView()
    {
        this.DataContext = new DesktopLicenseGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopLicenseGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}