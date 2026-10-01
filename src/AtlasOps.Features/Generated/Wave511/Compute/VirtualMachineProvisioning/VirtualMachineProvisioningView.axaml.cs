namespace AtlasOps.Features.Compute.VirtualMachineProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class VirtualMachineProvisioningView : UserControl
{
    public VirtualMachineProvisioningView()
    {
        this.DataContext = new VirtualMachineProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is VirtualMachineProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}