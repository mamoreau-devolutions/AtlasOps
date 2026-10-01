namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryDependencyRecoveryView : UserControl
{
    public RecoveryDependencyRecoveryView()
    {
        this.DataContext = new RecoveryDependencyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryDependencyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}