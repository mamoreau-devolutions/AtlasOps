namespace AtlasOps.Features.Cloud.CloudNetworkGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudNetworkGovernanceView : UserControl
{
    public CloudNetworkGovernanceView()
    {
        this.DataContext = new CloudNetworkGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudNetworkGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}