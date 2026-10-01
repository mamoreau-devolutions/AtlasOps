namespace AtlasOps.Features.Api.ApiHealthOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiHealthOptimizationView : UserControl
{
    public ApiHealthOptimizationView()
    {
        this.DataContext = new ApiHealthOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiHealthOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}