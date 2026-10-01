namespace AtlasOps.Features.Compute.VirtualMachineMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class VirtualMachineMonitoringView : UserControl
{
    public VirtualMachineMonitoringView()
    {
        this.DataContext = new VirtualMachineMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is VirtualMachineMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}