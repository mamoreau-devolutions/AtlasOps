namespace AtlasOps.Features.Compute.ComputeConsoleOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeConsoleOptimizationView : UserControl
{
    public ComputeConsoleOptimizationView()
    {
        this.DataContext = new ComputeConsoleOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeConsoleOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}