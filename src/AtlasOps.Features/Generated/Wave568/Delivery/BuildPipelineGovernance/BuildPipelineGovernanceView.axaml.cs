namespace AtlasOps.Features.Delivery.BuildPipelineGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildPipelineGovernanceView : UserControl
{
    public BuildPipelineGovernanceView()
    {
        this.DataContext = new BuildPipelineGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildPipelineGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}