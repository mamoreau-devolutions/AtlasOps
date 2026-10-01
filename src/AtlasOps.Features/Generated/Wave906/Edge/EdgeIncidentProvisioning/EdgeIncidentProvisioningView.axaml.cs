namespace AtlasOps.Features.Edge.EdgeIncidentProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeIncidentProvisioningView : UserControl
{
    public EdgeIncidentProvisioningView()
    {
        this.DataContext = new EdgeIncidentProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeIncidentProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}