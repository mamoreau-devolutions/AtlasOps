namespace AtlasOps.Features.Edge.EdgeUpdateProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeUpdateProvisioningView : UserControl
{
    public EdgeUpdateProvisioningView()
    {
        this.DataContext = new EdgeUpdateProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeUpdateProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}