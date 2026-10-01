namespace AtlasOps.Features.Compute.ComputeConsoleProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeConsoleProvisioningView : UserControl
{
    public ComputeConsoleProvisioningView()
    {
        this.DataContext = new ComputeConsoleProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeConsoleProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}