namespace AtlasOps.Features.Data.DataProductOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataProductOptimizationView : UserControl
{
    public DataProductOptimizationView()
    {
        this.DataContext = new DataProductOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataProductOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}