namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryDependencyOptimizationView : UserControl
{
    public RecoveryDependencyOptimizationView()
    {
        this.DataContext = new RecoveryDependencyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryDependencyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}