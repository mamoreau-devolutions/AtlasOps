namespace AtlasOps.Features.Mobile.MobileUpdateRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileUpdateRecoveryView : UserControl
{
    public MobileUpdateRecoveryView()
    {
        this.DataContext = new MobileUpdateRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileUpdateRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}