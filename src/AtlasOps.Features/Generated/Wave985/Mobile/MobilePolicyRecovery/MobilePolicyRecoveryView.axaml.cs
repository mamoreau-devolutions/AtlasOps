namespace AtlasOps.Features.Mobile.MobilePolicyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobilePolicyRecoveryView : UserControl
{
    public MobilePolicyRecoveryView()
    {
        this.DataContext = new MobilePolicyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobilePolicyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}