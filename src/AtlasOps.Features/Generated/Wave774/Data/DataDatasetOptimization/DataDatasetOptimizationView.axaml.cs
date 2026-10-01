namespace AtlasOps.Features.Data.DataDatasetOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataDatasetOptimizationView : UserControl
{
    public DataDatasetOptimizationView()
    {
        this.DataContext = new DataDatasetOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataDatasetOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}