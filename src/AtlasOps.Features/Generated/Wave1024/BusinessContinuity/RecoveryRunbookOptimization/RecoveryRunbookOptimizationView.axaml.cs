namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryRunbookOptimizationView : UserControl
{
    public RecoveryRunbookOptimizationView()
    {
        this.DataContext = new RecoveryRunbookOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryRunbookOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}