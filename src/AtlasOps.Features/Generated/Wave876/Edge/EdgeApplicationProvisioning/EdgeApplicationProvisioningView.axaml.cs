namespace AtlasOps.Features.Edge.EdgeApplicationProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeApplicationProvisioningView : UserControl
{
    public EdgeApplicationProvisioningView()
    {
        this.DataContext = new EdgeApplicationProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeApplicationProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}