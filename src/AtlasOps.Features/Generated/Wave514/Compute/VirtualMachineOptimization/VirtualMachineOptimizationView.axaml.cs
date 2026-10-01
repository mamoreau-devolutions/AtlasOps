namespace AtlasOps.Features.Compute.VirtualMachineOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class VirtualMachineOptimizationView : UserControl
{
    public VirtualMachineOptimizationView()
    {
        this.DataContext = new VirtualMachineOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is VirtualMachineOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}