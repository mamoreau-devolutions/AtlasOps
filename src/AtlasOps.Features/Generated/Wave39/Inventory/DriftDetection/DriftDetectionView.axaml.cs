namespace AtlasOps.Features.Inventory.DriftDetection;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DriftDetectionView : UserControl
{
    public DriftDetectionView()
    {
        this.DataContext = new DriftDetectionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DriftDetectionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}