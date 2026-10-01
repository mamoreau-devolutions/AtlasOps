namespace AtlasOps.Features.Delivery.BuildArtifactGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildArtifactGovernanceView : UserControl
{
    public BuildArtifactGovernanceView()
    {
        this.DataContext = new BuildArtifactGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildArtifactGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}