namespace AtlasOps.Features.Api.ApiGatewayOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiGatewayOptimizationView : UserControl
{
    public ApiGatewayOptimizationView()
    {
        this.DataContext = new ApiGatewayOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiGatewayOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}