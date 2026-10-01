namespace AtlasOps.Features.Delivery.BuildPipelineRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildPipelineRecoveryView : UserControl
{
    public BuildPipelineRecoveryView()
    {
        this.DataContext = new BuildPipelineRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildPipelineRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}