namespace AtlasOps.Features.Api.ApiContractOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiContractOptimizationView : UserControl
{
    public ApiContractOptimizationView()
    {
        this.DataContext = new ApiContractOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiContractOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}