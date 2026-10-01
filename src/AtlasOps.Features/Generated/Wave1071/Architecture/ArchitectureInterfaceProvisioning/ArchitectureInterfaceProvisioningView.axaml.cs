namespace AtlasOps.Features.Architecture.ArchitectureInterfaceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureInterfaceProvisioningView : UserControl
{
    public ArchitectureInterfaceProvisioningView()
    {
        this.DataContext = new ArchitectureInterfaceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureInterfaceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}