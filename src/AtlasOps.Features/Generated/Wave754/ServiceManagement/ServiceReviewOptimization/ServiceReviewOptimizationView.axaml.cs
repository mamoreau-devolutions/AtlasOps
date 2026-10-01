namespace AtlasOps.Features.ServiceManagement.ServiceReviewOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceReviewOptimizationView : UserControl
{
    public ServiceReviewOptimizationView()
    {
        this.DataContext = new ServiceReviewOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceReviewOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}