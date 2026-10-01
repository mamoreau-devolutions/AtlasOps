namespace AtlasOps.Features.Api.ApiAnalyticsOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiAnalyticsOptimizationView : UserControl
{
    public ApiAnalyticsOptimizationView()
    {
        this.DataContext = new ApiAnalyticsOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiAnalyticsOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}