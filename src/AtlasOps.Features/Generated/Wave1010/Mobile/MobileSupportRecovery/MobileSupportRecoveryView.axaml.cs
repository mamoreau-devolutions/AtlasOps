namespace AtlasOps.Features.Mobile.MobileSupportRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileSupportRecoveryView : UserControl
{
    public MobileSupportRecoveryView()
    {
        this.DataContext = new MobileSupportRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileSupportRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}