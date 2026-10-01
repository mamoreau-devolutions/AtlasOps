namespace AtlasOps.Features.Architecture.ArchitectureRoadmapGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRoadmapGovernanceView : UserControl
{
    public ArchitectureRoadmapGovernanceView()
    {
        this.DataContext = new ArchitectureRoadmapGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRoadmapGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}