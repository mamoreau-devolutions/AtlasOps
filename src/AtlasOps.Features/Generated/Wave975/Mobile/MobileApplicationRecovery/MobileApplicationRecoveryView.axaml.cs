namespace AtlasOps.Features.Mobile.MobileApplicationRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileApplicationRecoveryView : UserControl
{
    public MobileApplicationRecoveryView()
    {
        this.DataContext = new MobileApplicationRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileApplicationRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}