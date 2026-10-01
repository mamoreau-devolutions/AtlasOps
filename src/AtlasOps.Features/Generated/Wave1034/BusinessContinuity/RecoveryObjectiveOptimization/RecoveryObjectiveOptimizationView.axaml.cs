namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryObjectiveOptimizationView : UserControl
{
    public RecoveryObjectiveOptimizationView()
    {
        this.DataContext = new RecoveryObjectiveOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryObjectiveOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}