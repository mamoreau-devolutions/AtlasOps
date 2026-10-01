namespace AtlasOps.Features.Compute.ComputeImageProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeImageProvisioningView : UserControl
{
    public ComputeImageProvisioningView()
    {
        this.DataContext = new ComputeImageProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeImageProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}