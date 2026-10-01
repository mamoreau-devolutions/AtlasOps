namespace AtlasOps.Features.Data.DataContractOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataContractOptimizationView : UserControl
{
    public DataContractOptimizationView()
    {
        this.DataContext = new DataContractOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataContractOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}