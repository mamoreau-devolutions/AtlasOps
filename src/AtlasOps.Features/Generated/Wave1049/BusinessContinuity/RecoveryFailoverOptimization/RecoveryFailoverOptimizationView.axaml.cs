namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryFailoverOptimizationView : UserControl
{
    public RecoveryFailoverOptimizationView()
    {
        this.DataContext = new RecoveryFailoverOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryFailoverOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}