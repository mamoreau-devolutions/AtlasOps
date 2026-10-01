namespace AtlasOps.Features.Hardening.RecoveryMode;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryModeView : UserControl
{
    public RecoveryModeView()
    {
        this.DataContext = new RecoveryModeViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryModeViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}