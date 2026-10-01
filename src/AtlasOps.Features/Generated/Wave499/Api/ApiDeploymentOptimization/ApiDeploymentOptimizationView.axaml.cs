namespace AtlasOps.Features.Api.ApiDeploymentOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiDeploymentOptimizationView : UserControl
{
    public ApiDeploymentOptimizationView()
    {
        this.DataContext = new ApiDeploymentOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiDeploymentOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}