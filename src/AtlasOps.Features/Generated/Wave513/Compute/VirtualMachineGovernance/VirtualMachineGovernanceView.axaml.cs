namespace AtlasOps.Features.Compute.VirtualMachineGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class VirtualMachineGovernanceView : UserControl
{
    public VirtualMachineGovernanceView()
    {
        this.DataContext = new VirtualMachineGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is VirtualMachineGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}