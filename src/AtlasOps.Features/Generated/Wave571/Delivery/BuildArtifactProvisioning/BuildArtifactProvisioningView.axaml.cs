namespace AtlasOps.Features.Delivery.BuildArtifactProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildArtifactProvisioningView : UserControl
{
    public BuildArtifactProvisioningView()
    {
        this.DataContext = new BuildArtifactProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildArtifactProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}