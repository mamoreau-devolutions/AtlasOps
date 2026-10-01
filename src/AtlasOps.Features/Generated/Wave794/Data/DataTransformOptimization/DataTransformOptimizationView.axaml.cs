namespace AtlasOps.Features.Data.DataTransformOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataTransformOptimizationView : UserControl
{
    public DataTransformOptimizationView()
    {
        this.DataContext = new DataTransformOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataTransformOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}