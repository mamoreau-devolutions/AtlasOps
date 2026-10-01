namespace AtlasOps.Features.Edge.EdgePolicyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgePolicyProvisioningView : UserControl
{
    public EdgePolicyProvisioningView()
    {
        this.DataContext = new EdgePolicyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgePolicyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}