namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryObjectiveRecoveryView : UserControl
{
    public RecoveryObjectiveRecoveryView()
    {
        this.DataContext = new RecoveryObjectiveRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryObjectiveRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}