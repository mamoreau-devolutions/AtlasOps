namespace AtlasOps.Features.FinOps.ResourceCommitmentOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResourceCommitmentOptimizationView : UserControl
{
    public ResourceCommitmentOptimizationView()
    {
        this.DataContext = new ResourceCommitmentOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResourceCommitmentOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}