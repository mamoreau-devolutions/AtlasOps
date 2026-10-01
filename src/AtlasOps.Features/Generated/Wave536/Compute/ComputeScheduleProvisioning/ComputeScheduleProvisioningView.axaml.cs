namespace AtlasOps.Features.Compute.ComputeScheduleProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScheduleProvisioningView : UserControl
{
    public ComputeScheduleProvisioningView()
    {
        this.DataContext = new ComputeScheduleProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScheduleProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}