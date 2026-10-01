namespace AtlasOps.Features.Delivery.ReleaseEnvironmentProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseEnvironmentProvisioningView : UserControl
{
    public ReleaseEnvironmentProvisioningView()
    {
        this.DataContext = new ReleaseEnvironmentProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseEnvironmentProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}