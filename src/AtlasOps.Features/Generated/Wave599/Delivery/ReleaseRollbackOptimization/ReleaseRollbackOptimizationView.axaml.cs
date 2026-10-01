namespace AtlasOps.Features.Delivery.ReleaseRollbackOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseRollbackOptimizationView : UserControl
{
    public ReleaseRollbackOptimizationView()
    {
        this.DataContext = new ReleaseRollbackOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseRollbackOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}