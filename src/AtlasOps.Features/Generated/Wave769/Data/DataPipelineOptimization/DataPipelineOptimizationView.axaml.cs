namespace AtlasOps.Features.Data.DataPipelineOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataPipelineOptimizationView : UserControl
{
    public DataPipelineOptimizationView()
    {
        this.DataContext = new DataPipelineOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataPipelineOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}