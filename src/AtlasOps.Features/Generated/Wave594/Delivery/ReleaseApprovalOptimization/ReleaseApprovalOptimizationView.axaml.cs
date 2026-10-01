namespace AtlasOps.Features.Delivery.ReleaseApprovalOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseApprovalOptimizationView : UserControl
{
    public ReleaseApprovalOptimizationView()
    {
        this.DataContext = new ReleaseApprovalOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseApprovalOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}