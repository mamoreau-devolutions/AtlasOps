namespace AtlasOps.Features.Api.ApiVersionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiVersionOptimizationView : UserControl
{
    public ApiVersionOptimizationView()
    {
        this.DataContext = new ApiVersionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiVersionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}