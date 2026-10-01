namespace AtlasOps.Features.Cloud.GcpProjectProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class GcpProjectProvisioningView : UserControl
{
    public GcpProjectProvisioningView()
    {
        this.DataContext = new GcpProjectProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is GcpProjectProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}