namespace AtlasOps.Features.Delivery.SourceRepositoryOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SourceRepositoryOptimizationView : UserControl
{
    public SourceRepositoryOptimizationView()
    {
        this.DataContext = new SourceRepositoryOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SourceRepositoryOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}