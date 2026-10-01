namespace AtlasOps.Features.Delivery.BuildArtifactRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildArtifactRecoveryView : UserControl
{
    public BuildArtifactRecoveryView()
    {
        this.DataContext = new BuildArtifactRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildArtifactRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}