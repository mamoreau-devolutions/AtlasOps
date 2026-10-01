namespace AtlasOps.Features.Architecture.ArchitectureComponentProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureComponentProvisioningView : UserControl
{
    public ArchitectureComponentProvisioningView()
    {
        this.DataContext = new ArchitectureComponentProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureComponentProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}