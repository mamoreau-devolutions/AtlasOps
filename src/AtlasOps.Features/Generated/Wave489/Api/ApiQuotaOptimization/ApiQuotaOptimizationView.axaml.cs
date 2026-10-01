namespace AtlasOps.Features.Api.ApiQuotaOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiQuotaOptimizationView : UserControl
{
    public ApiQuotaOptimizationView()
    {
        this.DataContext = new ApiQuotaOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiQuotaOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}