namespace AtlasOps.Features.Compute.ComputePatchProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputePatchProvisioningView : UserControl
{
    public ComputePatchProvisioningView()
    {
        this.DataContext = new ComputePatchProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputePatchProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}