namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryRunbookRecoveryView : UserControl
{
    public RecoveryRunbookRecoveryView()
    {
        this.DataContext = new RecoveryRunbookRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryRunbookRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}