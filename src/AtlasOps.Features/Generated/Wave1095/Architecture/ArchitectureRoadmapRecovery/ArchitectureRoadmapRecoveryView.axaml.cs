namespace AtlasOps.Features.Architecture.ArchitectureRoadmapRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRoadmapRecoveryView : UserControl
{
    public ArchitectureRoadmapRecoveryView()
    {
        this.DataContext = new ArchitectureRoadmapRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRoadmapRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}