namespace AtlasOps.Features.Compute.ComputeImageOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeImageOptimizationView : UserControl
{
    public ComputeImageOptimizationView()
    {
        this.DataContext = new ComputeImageOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeImageOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}