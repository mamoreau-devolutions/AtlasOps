namespace AtlasOps.Features.Edge.EdgeSiteProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeSiteProvisioningView : UserControl
{
    public EdgeSiteProvisioningView()
    {
        this.DataContext = new EdgeSiteProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeSiteProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}