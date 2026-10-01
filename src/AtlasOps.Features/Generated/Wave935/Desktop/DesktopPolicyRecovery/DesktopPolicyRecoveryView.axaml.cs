namespace AtlasOps.Features.Desktop.DesktopPolicyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPolicyRecoveryView : UserControl
{
    public DesktopPolicyRecoveryView()
    {
        this.DataContext = new DesktopPolicyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPolicyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}