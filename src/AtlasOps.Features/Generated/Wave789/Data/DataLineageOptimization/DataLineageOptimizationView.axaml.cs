namespace AtlasOps.Features.Data.DataLineageOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataLineageOptimizationView : UserControl
{
    public DataLineageOptimizationView()
    {
        this.DataContext = new DataLineageOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataLineageOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}