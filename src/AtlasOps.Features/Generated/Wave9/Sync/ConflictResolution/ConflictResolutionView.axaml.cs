namespace AtlasOps.Features.Sync.ConflictResolution;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ConflictResolutionView : UserControl
{
    public ConflictResolutionView()
    {
        this.DataContext = new ConflictResolutionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ConflictResolutionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}