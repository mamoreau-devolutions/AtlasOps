namespace AtlasOps.Features.Compute.ComputeTemplateOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeTemplateOptimizationView : UserControl
{
    public ComputeTemplateOptimizationView()
    {
        this.DataContext = new ComputeTemplateOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeTemplateOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}