namespace AtlasOps.Features.Api.ApiTokenOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiTokenOptimizationView : UserControl
{
    public ApiTokenOptimizationView()
    {
        this.DataContext = new ApiTokenOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiTokenOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}