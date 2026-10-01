namespace AtlasOps.Features.Compute.VirtualMachineRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class VirtualMachineRecoveryView : UserControl
{
    public VirtualMachineRecoveryView()
    {
        this.DataContext = new VirtualMachineRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is VirtualMachineRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}