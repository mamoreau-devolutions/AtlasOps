namespace AtlasOps.Features.Data.DataAccessOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataAccessOptimizationView : UserControl
{
    public DataAccessOptimizationView()
    {
        this.DataContext = new DataAccessOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataAccessOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}