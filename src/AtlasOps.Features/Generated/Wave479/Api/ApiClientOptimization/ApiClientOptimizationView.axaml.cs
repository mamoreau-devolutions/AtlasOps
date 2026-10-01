namespace AtlasOps.Features.Api.ApiClientOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiClientOptimizationView : UserControl
{
    public ApiClientOptimizationView()
    {
        this.DataContext = new ApiClientOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiClientOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}