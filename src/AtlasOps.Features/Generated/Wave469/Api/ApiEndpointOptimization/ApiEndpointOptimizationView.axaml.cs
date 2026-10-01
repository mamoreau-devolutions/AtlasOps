namespace AtlasOps.Features.Api.ApiEndpointOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiEndpointOptimizationView : UserControl
{
    public ApiEndpointOptimizationView()
    {
        this.DataContext = new ApiEndpointOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiEndpointOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}