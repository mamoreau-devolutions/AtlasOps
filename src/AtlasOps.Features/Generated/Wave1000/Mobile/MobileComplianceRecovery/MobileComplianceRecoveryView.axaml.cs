namespace AtlasOps.Features.Mobile.MobileComplianceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileComplianceRecoveryView : UserControl
{
    public MobileComplianceRecoveryView()
    {
        this.DataContext = new MobileComplianceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileComplianceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}