namespace AtlasOps.Features.Mobile.MobileProfileRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileProfileRecoveryView : UserControl
{
    public MobileProfileRecoveryView()
    {
        this.DataContext = new MobileProfileRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileProfileRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}