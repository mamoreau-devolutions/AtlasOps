namespace AtlasOps.Features.BusinessContinuity.RecoverySiteOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoverySiteOptimizationView : UserControl
{
    public RecoverySiteOptimizationView()
    {
        this.DataContext = new RecoverySiteOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoverySiteOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}