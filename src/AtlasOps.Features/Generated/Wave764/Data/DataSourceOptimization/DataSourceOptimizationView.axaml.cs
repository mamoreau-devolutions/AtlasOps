namespace AtlasOps.Features.Data.DataSourceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataSourceOptimizationView : UserControl
{
    public DataSourceOptimizationView()
    {
        this.DataContext = new DataSourceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataSourceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}