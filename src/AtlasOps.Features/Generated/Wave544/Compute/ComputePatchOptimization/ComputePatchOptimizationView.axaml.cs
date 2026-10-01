namespace AtlasOps.Features.Compute.ComputePatchOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputePatchOptimizationView : UserControl
{
    public ComputePatchOptimizationView()
    {
        this.DataContext = new ComputePatchOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputePatchOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}