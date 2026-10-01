namespace AtlasOps.Features.Delivery.BuildPipelineProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildPipelineProvisioningView : UserControl
{
    public BuildPipelineProvisioningView()
    {
        this.DataContext = new BuildPipelineProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildPipelineProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}