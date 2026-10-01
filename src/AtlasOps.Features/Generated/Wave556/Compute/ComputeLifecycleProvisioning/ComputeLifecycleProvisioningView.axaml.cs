namespace AtlasOps.Features.Compute.ComputeLifecycleProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeLifecycleProvisioningView : UserControl
{
    public ComputeLifecycleProvisioningView()
    {
        this.DataContext = new ComputeLifecycleProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeLifecycleProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}