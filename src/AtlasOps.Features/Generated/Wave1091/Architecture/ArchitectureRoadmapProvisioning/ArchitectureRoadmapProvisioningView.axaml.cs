namespace AtlasOps.Features.Architecture.ArchitectureRoadmapProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRoadmapProvisioningView : UserControl
{
    public ArchitectureRoadmapProvisioningView()
    {
        this.DataContext = new ArchitectureRoadmapProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRoadmapProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}