namespace AtlasOps.Features.Compute.ComputeScaleSetProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScaleSetProvisioningView : UserControl
{
    public ComputeScaleSetProvisioningView()
    {
        this.DataContext = new ComputeScaleSetProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScaleSetProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}