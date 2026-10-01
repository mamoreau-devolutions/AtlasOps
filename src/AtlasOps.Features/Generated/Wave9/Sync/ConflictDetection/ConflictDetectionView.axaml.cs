namespace AtlasOps.Features.Sync.ConflictDetection;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ConflictDetectionView : UserControl
{
    public ConflictDetectionView()
    {
        this.DataContext = new ConflictDetectionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ConflictDetectionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}