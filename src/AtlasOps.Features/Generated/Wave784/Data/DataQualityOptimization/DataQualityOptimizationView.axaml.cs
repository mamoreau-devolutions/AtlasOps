namespace AtlasOps.Features.Data.DataQualityOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataQualityOptimizationView : UserControl
{
    public DataQualityOptimizationView()
    {
        this.DataContext = new DataQualityOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataQualityOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}