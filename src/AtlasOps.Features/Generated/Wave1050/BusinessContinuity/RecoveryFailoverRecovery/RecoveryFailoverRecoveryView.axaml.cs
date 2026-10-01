namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryFailoverRecoveryView : UserControl
{
    public RecoveryFailoverRecoveryView()
    {
        this.DataContext = new RecoveryFailoverRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryFailoverRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}